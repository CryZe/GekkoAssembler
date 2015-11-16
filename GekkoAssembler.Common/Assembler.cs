using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GekkoAssembler.DataSections;
using GekkoAssembler.GekkoInstructions;
using GekkoAssembler.IntermediateRepresentation;
using GekkoAssembler.Optimizers;

namespace GekkoAssembler
{
    public class Assembler
    {
        private static readonly Dictionary<string, Func<string[], int, GekkoInstruction>> instructionTable = new Dictionary<string, Func<string[], int, GekkoInstruction>>
        {
            {"add"    , ParseInstructionADD    },
            {"add."   , ParseInstructionADD    },
            {"addo"   , ParseInstructionADD    },
            {"addo."  , ParseInstructionADD    },
            {"addc"   , ParseInstructionADDC   },
            {"addc."  , ParseInstructionADDC   },
            {"addco"  , ParseInstructionADDC   },
            {"addco." , ParseInstructionADDC   },
            {"adde"   , ParseInstructionADDE   },
            {"adde."  , ParseInstructionADDE   },
            {"addeo"  , ParseInstructionADDE   },
            {"addeo." , ParseInstructionADDE   },
            {"addi"   , ParseInstructionADDI   },
            {"addic"  , ParseInstructionADDIC  },
            {"addic." , ParseInstructionADDIC  },
            {"addis"  , ParseInstructionADDIS  },
            {"addme"  , ParseInstructionADDME  },
            {"addme." , ParseInstructionADDME  },
            {"addmeo" , ParseInstructionADDME  },
            {"addmeo.", ParseInstructionADDME  },
            {"addze"  , ParseInstructionADDZE  },
            {"addze." , ParseInstructionADDZE  },
            {"addzeo" , ParseInstructionADDZE  },
            {"addzeo.", ParseInstructionADDZE  },
            {"and"    , ParseInstructionAND    },
            {"and."   , ParseInstructionAND    },
            {"b"      , ParseInstructionB      },
            {"ba"     , ParseInstructionBA     },
            {"bl"     , ParseInstructionBL     },
            {"bla"    , ParseInstructionBLA    },
            {"blr"    , ParseInstructionBLR    },
            {"crand"  , ParseInstructionCRAND  },
            {"crandc" , ParseInstructionCRANDC },
            {"crclr"  , ParseInstructionCRCLR  },
            {"creqv"  , ParseInstructionCREQV  },
            {"crmove" , ParseInstructionCRMOVE },
            {"crnand" , ParseInstructionCRNAND },
            {"crnor"  , ParseInstructionCRNOR  },
            {"crnot"  , ParseInstructionCRNOT  },
            {"cror"   , ParseInstructionCROR   },
            {"crorc"  , ParseInstructionCRORC  },
            {"crset"  , ParseInstructionCRSET  },
            {"crxor"  , ParseInstructionCRXOR  },
            {"divw"   , ParseInstructionDIVW   },
            {"icbi"   , ParseInstructionICBI   },
            {"isync"  , ParseInstructionISYNC  },
            {"lbz"    , ParseInstructionLBZ    },
            {"lfs"    , ParseInstructionLFS    },
            {"lhz"    , ParseInstructionLHZ    },
            {"lis"    , ParseInstructionLIS    },
            {"lwz"    , ParseInstructionLWZ    },
            {"mflr"   , ParseInstructionMFLR   },
            {"mfspr"  , ParseInstructionMFSPR  },
            {"mtlr"   , ParseInstructionMTLR   },
            {"mtspr"  , ParseInstructionMTSPR  },
            {"mulli"  , ParseInstructionMULLI  },
            {"mullw"  , ParseInstructionMULLW  },
            {"nop"    , ParseInstructionNOP    },
            {"ori"    , ParseInstructionORI    },
            {"stw"    , ParseInstructionSTW    },
            {"stwu"   , ParseInstructionSTWU   },
            {"sub"    , ParseInstructionSUB    },
            {"subic"  , ParseInstructionADDIC  }, // Simplified mnemonic for addic
            {"subic." , ParseInstructionADDIC  }, // Simplified mnemonic for addic.
            {"subf"   , ParseInstructionSUBF   }
        };

        public List<IOptimizer> Optimizers { get; }

        public Assembler(IEnumerable<IOptimizer> optimizers)
        {
            Optimizers = new List<IOptimizer>(optimizers);
        }

        public Assembler()
        {
            Optimizers = new List<IOptimizer>() { new GeneralOptimizer() };
        }

        private static string reduceLineToCode(string line)
        {
            if (line.Contains(";"))
                line = line.Substring(0, line.IndexOf(";"));

            return line.Trim();
        }

        public IRCodeBlock AssembleAllLines(IEnumerable<string> lines)
        {
            var instructionPointer = 0x00000000;
            return assembleAllLinesRef(new Queue<string>(lines), ref instructionPointer);
        }

        private IEnumerable<string> GetCodeLines(Queue<string> lines)
        {
            while (lines.Any())
            {
                var line = lines.Dequeue();
                line = reduceLineToCode(line);
                if (!string.IsNullOrWhiteSpace(line))
                    yield return line;
            }
        }

        public IRCodeBlock assembleAllLines(Queue<string> lines, int instructionPointer)
        {
            return assembleAllLinesRef(lines, ref instructionPointer);
        }

        private IRCodeBlock assembleAllLinesRef(Queue<string> lines, ref int instructionPointer)
        {
            var units = new List<IIRUnit>();

            foreach (var line in GetCodeLines(lines))
            {
                if (line.EndsWith(":"))
                {
                    instructionPointer = ParseInstructionPointerLabel(line);
                }
                else if (line.StartsWith("!"))
                {
                    if (line == "!end")
                        break;

                    var specialInstruction = ParseSpecialInstruction(line.Substring(1), ref instructionPointer, lines);
                    units.Add(specialInstruction);
                }
                else if (line.StartsWith("."))
                {
                    var dataSection = ParseDataSection(line.Substring(1), instructionPointer);
                    units.Add(dataSection);
                    instructionPointer += dataSection.Length;
                }
                else
                {
                    //Align the instruction
                    instructionPointer = (instructionPointer + 3) & ~3;
                    var instruction = ParseInstruction(line, instructionPointer);
                    units.Add(instruction);
                    instructionPointer += 4;
                }
            }

            var block = units.ToCodeBlock();

            foreach (var optimizer in Optimizers)
            {
                block = optimizer.Optimize(block);
            }

            return block;
        }

        private IIRUnit ParseSpecialInstruction(string line, ref int instructionPointer, Queue<string> lines)
        {
            var tokens = TokenizeLine(line);

            switch (tokens[0])
            {
                case "u8mask":
                    return ParseUnsigned8Mask(tokens, instructionPointer, lines);
                case "u16mask":
                    return ParseUnsigned16Mask(tokens, instructionPointer, lines);

                case "u8equal":
                    return ParseUnsigned8Equal(tokens, instructionPointer, lines);
                case "u16equal":
                    return ParseUnsigned16Equal(tokens, instructionPointer, lines);
                case "u32equal":
                    return ParseUnsigned32Equal(tokens, instructionPointer, lines);
                case "s8equal":
                    return ParseSigned8Equal(tokens, instructionPointer, lines);
                case "s16equal":
                    return ParseSigned16Equal(tokens, instructionPointer, lines);
                case "s32equal":
                    return ParseSigned32Equal(tokens, instructionPointer, lines);
                case "f32equal":
                    return ParseFloat32Equal(tokens, instructionPointer, lines);

                case "u8unequal":
                    return ParseUnsigned8Unequal(tokens, instructionPointer, lines);
                case "u16unequal":
                    return ParseUnsigned16Unequal(tokens, instructionPointer, lines);
                case "u32unequal":
                    return ParseUnsigned32Unequal(tokens, instructionPointer, lines);
                case "s8unequal":
                    return ParseSigned8Unequal(tokens, instructionPointer, lines);
                case "s16unequal":
                    return ParseSigned16Unequal(tokens, instructionPointer, lines);
                case "s32unequal":
                    return ParseSigned32Unequal(tokens, instructionPointer, lines);
                case "f32unequal":
                    return ParseFloat32Unequal(tokens, instructionPointer, lines);

                case "u8lessthan":
                    return ParseUnsigned8LessThan(tokens, instructionPointer, lines);
                case "u16lessthan":
                    return ParseUnsigned16LessThan(tokens, instructionPointer, lines);
                case "u32lessthan":
                    return ParseUnsigned32LessThan(tokens, instructionPointer, lines);
                case "s8lessthan":
                    return ParseSigned8LessThan(tokens, instructionPointer, lines);
                case "s16lessthan":
                    return ParseSigned16LessThan(tokens, instructionPointer, lines);
                case "s32lessthan":
                    return ParseSigned32LessThan(tokens, instructionPointer, lines);
                case "f32lessthan":
                    return ParseFloat32LessThan(tokens, instructionPointer, lines);

                case "u8greaterthan":
                    return ParseUnsigned8GreaterThan(tokens, instructionPointer, lines);
                case "u16greaterthan":
                    return ParseUnsigned16GreaterThan(tokens, instructionPointer, lines);
                case "u32greaterthan":
                    return ParseUnsigned32GreaterThan(tokens, instructionPointer, lines);
                case "s8greaterthan":
                    return ParseSigned8GreaterThan(tokens, instructionPointer, lines);
                case "s16greaterthan":
                    return ParseSigned16GreaterThan(tokens, instructionPointer, lines);
                case "s32greaterthan":
                    return ParseSigned32GreaterThan(tokens, instructionPointer, lines);
                case "f32greaterthan":
                    return ParseFloat32GreaterThan(tokens, instructionPointer, lines);

                case "u8add":
                    return ParseUnsigned8Add(tokens, ref instructionPointer);
                case "u16add":
                    return ParseUnsigned16Add(tokens, ref instructionPointer);
                case "u32add":
                    return ParseUnsigned32Add(tokens, ref instructionPointer);
                case "s8add":
                    return ParseSigned8Add(tokens, ref instructionPointer);
                case "s16add":
                    return ParseSigned16Add(tokens, ref instructionPointer);
                case "s32add":
                    return ParseSigned32Add(tokens, ref instructionPointer);
                case "f32add":
                    return ParseFloat32Add(tokens, ref instructionPointer);

                case "u8bitset":
                    return ParseUnsigned8BitSet(tokens, ref instructionPointer);
                case "u16bitset":
                    return ParseUnsigned16BitSet(tokens, ref instructionPointer);
                case "u32bitset":
                    return ParseUnsigned32BitSet(tokens, ref instructionPointer);

                case "u8bitunset":
                    return ParseUnsigned8BitUnset(tokens, ref instructionPointer);
                case "u16bitunset":
                    return ParseUnsigned16BitUnset(tokens, ref instructionPointer);
                case "u32bitunset":
                    return ParseUnsigned32BitUnset(tokens, ref instructionPointer);

                case "repeat":
                    return ParseRepeat(tokens, ref instructionPointer, lines);
            }

            throw new ArgumentException($"The specified special instruction { tokens[0] } is not supported.");
        }

        private IIRUnit ParseRepeat(string[] tokens, ref int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var units = new List<IIRUnit>();
            for (var i = 0; i < value - 1; ++i)
            {
                units.AddRange(assembleAllLinesRef(new Queue<string>(lines), ref instructionPointer).Units);
            }
            var lastRepeat = assembleAllLinesRef(lines, ref instructionPointer).Units;
            if (value > 0)
            {
                units.AddRange(lastRepeat);
            }
            return new IRMultiUnit(units);
        }

        #region Mask

        private IIRUnit ParseUnsigned8Mask(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Mask(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Mask(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Mask(instructionPointer, value, block);
        }

        #endregion

        #region Setting Bits

        private IIRUnit ParseUnsigned8BitSet(string[] tokens, ref int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned8BitSet(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16BitSet(string[] tokens, ref int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned16BitSet(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32BitSet(string[] tokens, ref int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned32BitSet(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Unsetting Bits

        private IIRUnit ParseUnsigned8BitUnset(string[] tokens, ref int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned8BitUnset(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16BitUnset(string[] tokens, ref int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned16BitUnset(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32BitUnset(string[] tokens, ref int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned32BitUnset(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Add

        private IIRUnit ParseUnsigned8Add(string[] tokens, ref int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned8Add(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16Add(string[] tokens, ref int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned16Add(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32Add(string[] tokens, ref int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        private IIRUnit ParseSigned8Add(string[] tokens, ref int instructionPointer)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var result = new IRSigned8Add(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseSigned16Add(string[] tokens, ref int instructionPointer)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var result = new IRSigned16Add(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseSigned32Add(string[] tokens, ref int instructionPointer)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var result = new IRSigned32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        private IIRUnit ParseFloat32Add(string[] tokens, ref int instructionPointer)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var result = new IRFloat32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Equal

        private IIRUnit ParseUnsigned8Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32Equal(instructionPointer, value, block);
        }

        #endregion

        #region Unequal

        private IIRUnit ParseUnsigned8Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32Unequal(instructionPointer, value, block);
        }

        #endregion

        #region Less Than

        private IIRUnit ParseUnsigned8LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32LessThan(instructionPointer, value, block);
        }

        #endregion

        #region Greater Than

        private IIRUnit ParseUnsigned8GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32GreaterThan(instructionPointer, value, block);
        }

        #endregion

        private IRWriteData ParseDataSection(string line, int instructionPointer)
        {
            var tokens = TokenizeLine(line);

            switch (tokens[0])
            {
                case "str":
                    return ParseStringDataSection(line, instructionPointer);
                case "u8":
                    return ParseUnsigned8DataSection(tokens, instructionPointer);
                case "u16":
                    return ParseUnsigned16DataSection(tokens, instructionPointer);
                case "u32":
                    return ParseUnsigned32DataSection(tokens, instructionPointer);
                case "u64":
                    return ParseUnsigned64DataSection(tokens, instructionPointer);
                case "s8":
                    return ParseSigned8DataSection(tokens, instructionPointer);
                case "s16":
                    return ParseSigned16DataSection(tokens, instructionPointer);
                case "s32":
                    return ParseSigned32DataSection(tokens, instructionPointer);
                case "s64":
                    return ParseSigned64DataSection(tokens, instructionPointer);
                case "f32":
                    return ParseFloat32DataSection(tokens, instructionPointer);
                case "f64":
                    return ParseFloat64DataSection(tokens, instructionPointer);
            }

            throw new ArgumentException($"The specified data section { tokens[0] } is not supported.");
        }

        #region Data Sections

        private IRWriteData ParseStringDataSection(string line, int instructionPointer)
        {
            var literal = line.Substring(line.IndexOf("\""));
            var text = ParseStringLiteral(literal);
            return new StringDataSection(instructionPointer, text);
        }

        private string ParseStringLiteral(string literal)
        {
            return literal.Substring(1, literal.Length - 2);
        }

        private IRWriteData ParseUnsigned8DataSection(string[] tokens, int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            return new Unsigned8DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned16DataSection(string[] tokens, int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            return new Unsigned16DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned32DataSection(string[] tokens, int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            return new Unsigned32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned64DataSection(string[] tokens, int instructionPointer)
        {
            var value = (ulong)ParseInteger64Literal(tokens[1]);
            return new Unsigned64DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned8DataSection(string[] tokens, int instructionPointer)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            return new Signed8DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned16DataSection(string[] tokens, int instructionPointer)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            return new Signed16DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned32DataSection(string[] tokens, int instructionPointer)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            return new Signed32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned64DataSection(string[] tokens, int instructionPointer)
        {
            var value = ParseInteger64Literal(tokens[1]);
            return new Signed64DataSection(instructionPointer, value);
        }

        private IRWriteData ParseFloat32DataSection(string[] tokens, int instructionPointer)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            return new Float32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseFloat64DataSection(string[] tokens, int instructionPointer)
        {
            var value = ParseFloatLiteral(tokens[1]);
            return new Float64DataSection(instructionPointer, value);
        }

        #endregion

        private static GekkoInstruction ParseInstruction(string line, int instructionPointer)
        {
            var tokens = TokenizeLine(line);

            // Lowercase the instruction name.
            // Handles stylistic forms where someone might
            // prefer uppercasing their assembly instructions.
            tokens[0] = tokens[0].ToLower();

            if (instructionTable.ContainsKey(tokens[0]))
                return instructionTable[tokens[0]](tokens, instructionPointer);

            throw new ArgumentException($"The specified instruction { tokens[0] } is not supported.");
        }

        #region Gekko Instructions

        private static GekkoInstruction ParseInstructionADD(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new AddInstruction(instructionPointer, rd, ra, rb, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDC(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new AddCarryingInstruction(instructionPointer, rd, ra, rb, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDE(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new AddExtendedInstruction(instructionPointer, rd, ra, rb, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDI(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var simm = ParseIntegerLiteral(tokens[3]);
            return new AddImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private static GekkoInstruction ParseInstructionADDME(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            return new AddToMinusOneExtendedInstruction(instructionPointer, rd, ra, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDZE(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            return new AddToZeroExtendedInstruction(instructionPointer, rd, ra, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDIC(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            int simm = ParseIntegerLiteral(tokens[3]);

            if (tokens[0] == "addic")
                return new AddImmediateCarryingInstruction(instructionPointer, rd, ra, simm);
            if (tokens[0] == "addic.")
                return new AddImmediateCarryingAndRecordInstruction(instructionPointer, rd, ra, simm);

            // subic mnemonics
            if (tokens[0] == "subic")
                return new AddImmediateCarryingInstruction(instructionPointer, rd, ra, -simm);
            // subic.
            return new AddImmediateCarryingAndRecordInstruction(instructionPointer, rd, ra, -simm);
        }

        private static GekkoInstruction ParseInstructionADDIS(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var simm = ParseIntegerLiteral(tokens[3]);
            return new AddImmediateShiftedInstruction(instructionPointer, rd, ra, simm);
        }

        private static GekkoInstruction ParseInstructionAND(string[] tokens, int instructionPointer)
        {
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new AndInstruction(instructionPointer, ra, rs, rb, tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionB(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, false, false);
        }

        private static GekkoInstruction ParseInstructionBA(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, true, false);
        }

        private static GekkoInstruction ParseInstructionBL(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, false, true);
        }

        private static GekkoInstruction ParseInstructionBLA(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, true, true);
        }

        private static GekkoInstruction ParseInstructionBLR(string[] tokens, int instructionPointer)
        {
            return new BranchToLinkRegisterInstruction(instructionPointer);
        }

        private static GekkoInstruction ParseInstructionCRAND(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterANDInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRANDC(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterANDComplementInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRCLR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            return new ConditionRegisterClearInstruction(instructionPointer, crbd);
        }

        private static GekkoInstruction ParseInstructionCREQV(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterEquivalentInstruction(instructionPointer, crbd, crba, crbb);
        }

        // Simplified mnemonic of CROR
        private static GekkoInstruction ParseInstructionCRMOVE(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            return new ConditionRegisterORInstruction(instructionPointer, crbd, crba, crba);
        }

        private static GekkoInstruction ParseInstructionCRNAND(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterNANDInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRNOR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterNORInstruction(instructionPointer, crbd, crba, crbb);
        }

        // Simplified mnemonic of CRNOR
        private static GekkoInstruction ParseInstructionCRNOT(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            return new ConditionRegisterNORInstruction(instructionPointer, crbd, crba, crba);
        }

        private static GekkoInstruction ParseInstructionCROR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRORC(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterORComplementInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRSET(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            return new ConditionRegisterSetInstruction(instructionPointer, crbd);
        }

        private static GekkoInstruction ParseInstructionCRXOR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterXORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionDIVW(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new DivideWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private static GekkoInstruction ParseInstructionICBI(string[] tokens, int instructionPointer)
        {
            var ra = ParseRegister(tokens[1]);
            var rb = ParseRegister(tokens[2]);
            return new InstructionCacheBlockInvalidateInstruction(instructionPointer, ra, rb);
        }

        private static GekkoInstruction ParseInstructionISYNC(string[] tokens, int instructionPointer)
        {
            return new InstructionSynchronizeInstruction(instructionPointer);
        }

        private static GekkoInstruction ParseInstructionLBZ(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra = ParseRegister(tokens[3]);
            return new LoadByteAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private static GekkoInstruction ParseInstructionLFS(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra = ParseRegister(tokens[3]);
            return new LoadFloatingPointSingleInstruction(instructionPointer, rd, ra, offset);
        }

        private static GekkoInstruction ParseInstructionLHZ(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra = ParseRegister(tokens[3]);
            return new LoadHalfWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private static GekkoInstruction ParseInstructionLIS(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var simm = ParseIntegerLiteral(tokens[2]);
            return new LoadImmediateShiftedInstruction(instructionPointer, rd, simm);
        }

        private static GekkoInstruction ParseInstructionLWZ(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra = ParseRegister(tokens[3]);
            return new LoadWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private static GekkoInstruction ParseInstructionMFLR(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            return new MoveFromLinkRegisterInstruction(instructionPointer, rd);
        }

        private static GekkoInstruction ParseInstructionMFSPR(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var spr = ParseIntegerLiteral(tokens[2]);
            return new MoveFromSpecialPurposeRegisterInstruction(instructionPointer, rd, spr);
        }

        private static GekkoInstruction ParseInstructionMTLR(string[] tokens, int instructionPointer)
        {
            var rs = ParseRegister(tokens[1]);
            return new MoveToLinkRegisterInstruction(instructionPointer, rs);
        }

        private static GekkoInstruction ParseInstructionMTSPR(string[] tokens, int instructionPointer)
        {
            var spr = ParseIntegerLiteral(tokens[1]);
            var rs = ParseRegister(tokens[2]);
            return new MoveToSpecialPurposeRegisterInstruction(instructionPointer, spr, rs);
        }

        private static GekkoInstruction ParseInstructionMULLI(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var simm = ParseIntegerLiteral(tokens[3]);
            return new MultiplyLowImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private static GekkoInstruction ParseInstructionMULLW(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new MultiplyLowWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private static GekkoInstruction ParseInstructionNOP(string[] tokens, int instructionPointer)
        {
            return new NoOperationInstruction(instructionPointer);
        }

        private static GekkoInstruction ParseInstructionORI(string[] tokens, int instructionPointer)
        {
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);
            var uimm = ParseIntegerLiteral(tokens[3]);
            return new OrImmediateInstruction(instructionPointer, ra, rs, uimm);
        }

        private static GekkoInstruction ParseInstructionSTW(string[] tokens, int instructionPointer)
        {
            var rs = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra = ParseRegister(tokens[3]);
            return new StoreWordInstruction(instructionPointer, rs, offset, ra);
        }

        private static GekkoInstruction ParseInstructionSTWU(string[] tokens, int instructionPointer)
        {
            var rs = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra = ParseRegister(tokens[3]);
            return new StoreWordWithUpdateInstruction(instructionPointer, rs, offset, ra);
        }

        private static GekkoInstruction ParseInstructionSUB(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new SubtractFromInstruction(instructionPointer, rd, rb, ra, false, false);
        }

        private static GekkoInstruction ParseInstructionSUBF(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new SubtractFromInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        #endregion

        private static int ParseRegister(string register)
        {
            string literal;

            if (register == "sp")
                return 1;

            if (register.StartsWith("r"))
                literal = register.Substring(1);
            else if (register.StartsWith("fr"))
                literal = register.Substring(2);
            else
                throw new ArgumentException("Not a valid register", nameof(register));

            int registerNumber = ParseIntegerLiteral(literal);

            if (registerNumber < 0 || registerNumber > 31)
                throw new ArgumentException("General-purpose registers and floating-point registers must be within the range 0-31");

            return registerNumber;
        }

        private static int ParseConditionRegister(string register)
        {
            int registerNumber = ParseIntegerLiteral(register.Substring(3));

            if (registerNumber < 0 || registerNumber > 7)
                throw new ArgumentException("Condition registers must be within the range 0-7");

            return registerNumber;
        }

        private static int ParseInstructionPointerLabel(string line)
        {
            var address = line.Remove(line.LastIndexOf(':'));
            return ParseIntegerLiteral(address);
        }

        private static long ParseInteger64Literal(string literal)
        {
            if (literal.Contains("0x"))
            {
                //Hexadecimal
                literal = literal.Replace("0x", "");
                var negative = literal.StartsWith("-");
                if (negative)
                    literal = literal.Substring(1);
                return (negative ? -1 : 1) * long.Parse(literal, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            else
            {
                return long.Parse(literal, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
        }

        private static int ParseIntegerLiteral(string literal)
        {
            return (int)ParseInteger64Literal(literal);
        }

        private static double ParseFloatLiteral(string literal)
        {
            return double.Parse(literal, CultureInfo.InvariantCulture);
        }

        private static string[] TokenizeLine(string line)
        {
            return Regex.Replace(line, "[\t,()]", " ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(token => token.Trim()).ToArray();
        }
    }
}
