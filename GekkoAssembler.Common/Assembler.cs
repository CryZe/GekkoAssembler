using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GekkoAssembler.DataSections;
using GekkoAssembler.GekkoInstructions;
using GekkoAssembler.IntermediateRepresentation;
using GekkoAssembler.Optimizers;

namespace GekkoAssembler
{
    public class Assembler
    {
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
            if (line.StartsWith("u8mask "))
                return ParseUnsigned8Mask(line, instructionPointer, lines);
            if (line.StartsWith("u16mask "))
                return ParseUnsigned16Mask(line, instructionPointer, lines);

            if (line.StartsWith("u8equal "))
                return ParseUnsigned8Equal(line, instructionPointer, lines);
            if (line.StartsWith("u16equal "))
                return ParseUnsigned16Equal(line, instructionPointer, lines);
            if (line.StartsWith("u32equal "))
                return ParseUnsigned32Equal(line, instructionPointer, lines);
            if (line.StartsWith("s8equal "))
                return ParseSigned8Equal(line, instructionPointer, lines);
            if (line.StartsWith("s16equal "))
                return ParseSigned16Equal(line, instructionPointer, lines);
            if (line.StartsWith("s32equal "))
                return ParseSigned32Equal(line, instructionPointer, lines);
            if (line.StartsWith("f32equal "))
                return ParseFloat32Equal(line, instructionPointer, lines);

            if (line.StartsWith("u8unequal "))
                return ParseUnsigned8Unequal(line, instructionPointer, lines);
            if (line.StartsWith("u16unequal "))
                return ParseUnsigned16Unequal(line, instructionPointer, lines);
            if (line.StartsWith("u32unequal "))
                return ParseUnsigned32Unequal(line, instructionPointer, lines);
            if (line.StartsWith("s8unequal "))
                return ParseSigned8Unequal(line, instructionPointer, lines);
            if (line.StartsWith("s16unequal "))
                return ParseSigned16Unequal(line, instructionPointer, lines);
            if (line.StartsWith("s32unequal "))
                return ParseSigned32Unequal(line, instructionPointer, lines);
            if (line.StartsWith("f32unequal "))
                return ParseFloat32Unequal(line, instructionPointer, lines);

            if (line.StartsWith("u8lessthan "))
                return ParseUnsigned8LessThan(line, instructionPointer, lines);
            if (line.StartsWith("u16lessthan "))
                return ParseUnsigned16LessThan(line, instructionPointer, lines);
            if (line.StartsWith("u32lessthan "))
                return ParseUnsigned32LessThan(line, instructionPointer, lines);
            if (line.StartsWith("s8lessthan "))
                return ParseSigned8LessThan(line, instructionPointer, lines);
            if (line.StartsWith("s16lessthan "))
                return ParseSigned16LessThan(line, instructionPointer, lines);
            if (line.StartsWith("s32lessthan "))
                return ParseSigned32LessThan(line, instructionPointer, lines);
            if (line.StartsWith("f32lessthan "))
                return ParseFloat32LessThan(line, instructionPointer, lines);

            if (line.StartsWith("u8greaterthan "))
                return ParseUnsigned8GreaterThan(line, instructionPointer, lines);
            if (line.StartsWith("u16greaterthan "))
                return ParseUnsigned16GreaterThan(line, instructionPointer, lines);
            if (line.StartsWith("u32greaterthan "))
                return ParseUnsigned32GreaterThan(line, instructionPointer, lines);
            if (line.StartsWith("s8greaterthan "))
                return ParseSigned8GreaterThan(line, instructionPointer, lines);
            if (line.StartsWith("s16greaterthan "))
                return ParseSigned16GreaterThan(line, instructionPointer, lines);
            if (line.StartsWith("s32greaterthan "))
                return ParseSigned32GreaterThan(line, instructionPointer, lines);
            if (line.StartsWith("f32greaterthan "))
                return ParseFloat32GreaterThan(line, instructionPointer, lines);

            if (line.StartsWith("u8add "))
                return ParseUnsigned8Add(line, ref instructionPointer);
            if (line.StartsWith("u16add "))
                return ParseUnsigned16Add(line, ref instructionPointer);
            if (line.StartsWith("u32add "))
                return ParseUnsigned32Add(line, ref instructionPointer);
            if (line.StartsWith("s8add "))
                return ParseSigned8Add(line, ref instructionPointer);
            if (line.StartsWith("s16add "))
                return ParseSigned16Add(line, ref instructionPointer);
            if (line.StartsWith("s32add "))
                return ParseSigned32Add(line, ref instructionPointer);
            if (line.StartsWith("f32add "))
                return ParseFloat32Add(line, ref instructionPointer);

            if (line.StartsWith("u8bitset "))
                return ParseUnsigned8BitSet(line, ref instructionPointer);
            if (line.StartsWith("u16bitset "))
                return ParseUnsigned16BitSet(line, ref instructionPointer);
            if (line.StartsWith("u32bitset "))
                return ParseUnsigned32BitSet(line, ref instructionPointer);

            if (line.StartsWith("repeat "))
                return ParseRepeat(line, ref instructionPointer, lines);

            throw new ArgumentException($"The specified special instruction { line } is not supported.");
        }

        private IIRUnit ParseRepeat(string line, ref int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "repeat");
            var value = ParseIntegerLiteral(parameters[0]);
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

        private IIRUnit ParseUnsigned8Mask(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u8mask");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Mask(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Mask(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u16mask");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Mask(instructionPointer, value, block);
        }

        #endregion

        #region Bit Set

        private IIRUnit ParseUnsigned8BitSet(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "u8bitset");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            var result = new IRUnsigned8BitSet(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16BitSet(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "u16bitset");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            var result = new IRUnsigned16BitSet(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32BitSet(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "u32bitset");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            var result = new IRUnsigned32BitSet(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Add

        private IIRUnit ParseUnsigned8Add(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "u8add");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            var result = new IRUnsigned8Add(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16Add(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "u16add");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            var result = new IRUnsigned16Add(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32Add(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "u32add");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            var result = new IRUnsigned32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        private IIRUnit ParseSigned8Add(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "s8add");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            var result = new IRSigned8Add(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseSigned16Add(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "s16add");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            var result = new IRSigned16Add(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseSigned32Add(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "s32add");
            var value = ParseIntegerLiteral(parameters[0]);
            var result = new IRSigned32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        private IIRUnit ParseFloat32Add(string line, ref int instructionPointer)
        {
            var parameters = ParseParameters(line, "f32add");
            var value = (float)ParseFloatLiteral(parameters[0]);
            var result = new IRFloat32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Equal

        private IIRUnit ParseUnsigned8Equal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u8equal");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Equal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u16equal");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32Equal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u32equal");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8Equal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s8equal");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16Equal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s16equal");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32Equal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s32equal");
            var value = ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32Equal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "f32equal");
            var value = (float)ParseFloatLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32Equal(instructionPointer, value, block);
        }

        #endregion

        #region Unequal

        private IIRUnit ParseUnsigned8Unequal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u8unequal");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Unequal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u16unequal");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32Unequal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u32unequal");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8Unequal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s8unequal");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16Unequal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s16unequal");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32Unequal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s32unequal");
            var value = ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32Unequal(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "f32unequal");
            var value = (float)ParseFloatLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32Unequal(instructionPointer, value, block);
        }

        #endregion

        #region Less Than

        private IIRUnit ParseUnsigned8LessThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u8lessthan");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16LessThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u16lessthan");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32LessThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u32lessthan");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8LessThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s8lessthan");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16LessThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s16lessthan");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32LessThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s32lessthan");
            var value = ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32LessThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "f32lessthan");
            var value = (float)ParseFloatLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32LessThan(instructionPointer, value, block);
        }

        #endregion

        #region Greater Than

        private IIRUnit ParseUnsigned8GreaterThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u8greaterthan");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16GreaterThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u16greaterthan");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32GreaterThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "u32greaterthan");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8GreaterThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s8greaterthan");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16GreaterThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s16greaterthan");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32GreaterThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "s32greaterthan");
            var value = ParseIntegerLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32GreaterThan(string line, int instructionPointer, Queue<string> lines)
        {
            var parameters = ParseParameters(line, "f32greaterthan");
            var value = (float)ParseFloatLiteral(parameters[0]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32GreaterThan(instructionPointer, value, block);
        }

        #endregion

        private IRWriteData ParseDataSection(string line, int instructionPointer)
        {
            if (line.StartsWith("str "))
                return ParseStringDataSection(line, instructionPointer);
            if (line.StartsWith("u8 "))
                return ParseUnsigned8DataSection(line, instructionPointer);
            if (line.StartsWith("u16 "))
                return ParseUnsigned16DataSection(line, instructionPointer);
            if (line.StartsWith("u32 "))
                return ParseUnsigned32DataSection(line, instructionPointer);
            if (line.StartsWith("u64 "))
                return ParseUnsigned64DataSection(line, instructionPointer);
            if (line.StartsWith("s8 "))
                return ParseSigned8DataSection(line, instructionPointer);
            if (line.StartsWith("s16 "))
                return ParseSigned16DataSection(line, instructionPointer);
            if (line.StartsWith("s32 "))
                return ParseSigned32DataSection(line, instructionPointer);
            if (line.StartsWith("s64 "))
                return ParseSigned64DataSection(line, instructionPointer);
            if (line.StartsWith("f32 "))
                return ParseFloat32DataSection(line, instructionPointer);
            if (line.StartsWith("f64 "))
                return ParseFloat64DataSection(line, instructionPointer);

            throw new ArgumentException($"The specified data section { line } is not supported.");
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

        private IRWriteData ParseUnsigned8DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u8");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            return new Unsigned8DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned16DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u16");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            return new Unsigned16DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned32DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u32");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            return new Unsigned32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned64DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u64");
            var value = (ulong)ParseInteger64Literal(parameters[0]);
            return new Unsigned64DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned8DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s8");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            return new Signed8DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned16DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s16");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            return new Signed16DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned32DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s32");
            var value = ParseIntegerLiteral(parameters[0]);
            return new Signed32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned64DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s64");
            var value = ParseInteger64Literal(parameters[0]);
            return new Signed64DataSection(instructionPointer, value);
        }

        private IRWriteData ParseFloat32DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "f32");
            var value = (float)ParseFloatLiteral(parameters[0]);
            return new Float32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseFloat64DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "f64");
            var value = ParseFloatLiteral(parameters[0]);
            return new Float64DataSection(instructionPointer, value);
        }

        #endregion

        private GekkoInstruction ParseInstruction(string line, int instructionPointer)
        {
            if (line.StartsWith("addi "))
                return ParseInstructionADDI(line, instructionPointer);
            if (line.StartsWith("addis "))
                return ParseInstructionADDIS(line, instructionPointer);
            if (line.StartsWith("b "))
                return ParseInstructionB(line, instructionPointer);
            if (line.StartsWith("ba "))
                return ParseInstructionBA(line, instructionPointer);
            if (line.StartsWith("bl "))
                return ParseInstructionBL(line, instructionPointer);
            if (line.StartsWith("bla "))
                return ParseInstructionBLA(line, instructionPointer);
            if (line.StartsWith("blr"))
                return ParseInstructionBLR(line, instructionPointer);
            if (line.StartsWith("crand "))
                return ParseInstructionCRAND(line, instructionPointer);
            if (line.StartsWith("crandc "))
                return ParseInstructionCRANDC(line, instructionPointer);
            if (line.StartsWith("crclr "))
                return ParseInstructionCRCLR(line, instructionPointer);
            if (line.StartsWith("creqv "))
                return ParseInstructionCREQV(line, instructionPointer);
            if (line.StartsWith("crmove "))
                return ParseInstructionCRMOVE(line, instructionPointer);
            if (line.StartsWith("crnand "))
                return ParseInstructionCRNAND(line, instructionPointer);
            if (line.StartsWith("crnor "))
                return ParseInstructionCRNOR(line, instructionPointer);
            if (line.StartsWith("crnot "))
                return ParseInstructionCRNOT(line, instructionPointer);
            if (line.StartsWith("cror "))
                return ParseInstructionCROR(line, instructionPointer);
            if (line.StartsWith("crorc "))
                return ParseInstructionCRORC(line, instructionPointer);
            if (line.StartsWith("crset "))
                return ParseInstructionCRSET(line, instructionPointer);
            if (line.StartsWith("crxor "))
                return ParseInstructionCRXOR(line, instructionPointer);
            if (line.StartsWith("divw "))
                return ParseInstructionDIVW(line, instructionPointer);
            if (line.StartsWith("lbz "))
                return ParseInstructionLBZ(line, instructionPointer);
            if (line.StartsWith("lfs "))
                return ParseInstructionLFS(line, instructionPointer);
            if (line.StartsWith("lhz "))
                return ParseInstructionLHZ(line, instructionPointer);
            if (line.StartsWith("lis "))
                return ParseInstructionLIS(line, instructionPointer);
            if (line.StartsWith("lwz "))
                return ParseInstructionLWZ(line, instructionPointer);
            if (line.StartsWith("mflr "))
                return ParseInstructionMFLR(line, instructionPointer);
            if (line.StartsWith("mfspr "))
                return ParseInstructionMFSPR(line, instructionPointer);
            if (line.StartsWith("mtlr "))
                return ParseInstructionMTLR(line, instructionPointer);
            if (line.StartsWith("mtspr "))
                return ParseInstructionMTSPR(line, instructionPointer);
            if (line.StartsWith("mulli "))
                return ParseInstructionMULLI(line, instructionPointer);
            if (line.StartsWith("mullw "))
                return ParseInstructionMULLW(line, instructionPointer);
            if (line.StartsWith("nop"))
                return ParseInstructionNOP(line, instructionPointer);
            if (line.StartsWith("ori "))
                return ParseInstructionORI(line, instructionPointer);
            if (line.StartsWith("stw "))
                return ParseInstructionSTW(line, instructionPointer);
            if (line.StartsWith("stwu "))
                return ParseInstructionSTWU(line, instructionPointer);
            if (line.StartsWith("sub "))
                return ParseInstructionSUB(line, instructionPointer);
            if (line.StartsWith("subf "))
                return ParseInstructionSUBF(line, instructionPointer);

            throw new ArgumentException($"The specified instruction { line } is not supported.");
        }

        #region Gekko Instructions

        private GekkoInstruction ParseInstructionADDI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "addi");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new AddImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private GekkoInstruction ParseInstructionADDIS(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "addis");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new AddImmediateShiftedInstruction(instructionPointer, rd, ra, simm);
        }

        private GekkoInstruction ParseInstructionB(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "b");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, false, false);
        }

        private GekkoInstruction ParseInstructionBA(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "ba");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, true, false);
        }

        private GekkoInstruction ParseInstructionBL(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "bl");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, false, true);
        }

        private GekkoInstruction ParseInstructionBLA(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "bla");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, true, true);
        }

        private GekkoInstruction ParseInstructionBLR(string line, int instructionPointer)
        {
            return new BranchToLinkRegisterInstruction(instructionPointer);
        }

        private GekkoInstruction ParseInstructionCRAND(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crand");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterANDInstruction(instructionPointer, crbd, crba, crbb);
        }

        private GekkoInstruction ParseInstructionCRANDC(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crandc");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterANDComplementInstruction(instructionPointer, crbd, crba, crbb);
        }

        private GekkoInstruction ParseInstructionCRCLR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crclr");
            var crbd = ParseConditionRegister(parameters[0]);
            return new ConditionRegisterClearInstruction(instructionPointer, crbd);
        }

        private GekkoInstruction ParseInstructionCREQV(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "creqv");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterEquivalentInstruction(instructionPointer, crbd, crba, crbb);
        }

        // Simplified mnemonic of CROR
        private GekkoInstruction ParseInstructionCRMOVE(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crmove");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            return new ConditionRegisterORInstruction(instructionPointer, crbd, crba, crba);
        }

        private GekkoInstruction ParseInstructionCRNAND(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crnand");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterNANDInstruction(instructionPointer, crbd, crba, crbb);
        }

        private GekkoInstruction ParseInstructionCRNOR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crnor");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterNORInstruction(instructionPointer, crbd, crba, crbb);
        }

        // Simplified mnemonic of CRNOR
        private GekkoInstruction ParseInstructionCRNOT(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crnot");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            return new ConditionRegisterNORInstruction(instructionPointer, crbd, crba, crba);
        }

        private GekkoInstruction ParseInstructionCROR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "cror");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private GekkoInstruction ParseInstructionCRORC(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crorc");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterORComplementInstruction(instructionPointer, crbd, crba, crbb);
        }

        private GekkoInstruction ParseInstructionCRSET(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crset");
            var crbd = ParseConditionRegister(parameters[0]);
            return new ConditionRegisterSetInstruction(instructionPointer, crbd);
        }

        private GekkoInstruction ParseInstructionCRXOR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crxor");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterXORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private GekkoInstruction ParseInstructionDIVW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "divw");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new DivideWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private GekkoInstruction ParseInstructionLBZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lbz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadByteAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private GekkoInstruction ParseInstructionLFS(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lfs");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadFloatingPointSingleInstruction(instructionPointer, rd, ra, offset);
        }

        private GekkoInstruction ParseInstructionLHZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lhz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadHalfWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private GekkoInstruction ParseInstructionLIS(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lis");
            var rd = ParseRegister(parameters[0]);
            var simm = ParseIntegerLiteral(parameters[1]);
            return new LoadImmediateShiftedInstruction(instructionPointer, rd, simm);
        }

        private GekkoInstruction ParseInstructionLWZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lwz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private GekkoInstruction ParseInstructionMFLR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mflr");
            var rd = ParseRegister(parameters[0]);
            return new MoveFromLinkRegisterInstruction(instructionPointer, rd);
        }

        private GekkoInstruction ParseInstructionMFSPR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mfspr");
            var rd = ParseRegister(parameters[0]);
            var spr = ParseIntegerLiteral(parameters[1]);
            return new MoveFromSpecialPurposeRegisterInstruction(instructionPointer, rd, spr);
        }

        private GekkoInstruction ParseInstructionMTLR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mtlr");
            var rs = ParseRegister(parameters[0]);
            return new MoveToLinkRegisterInstruction(instructionPointer, rs);
        }

        private GekkoInstruction ParseInstructionMTSPR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mtspr");
            var spr = ParseIntegerLiteral(parameters[0]);
            var rs = ParseRegister(parameters[1]);
            return new MoveToSpecialPurposeRegisterInstruction(instructionPointer, spr, rs);
        }

        private GekkoInstruction ParseInstructionMULLI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mulli");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new MultiplyLowImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private GekkoInstruction ParseInstructionMULLW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mullw");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new MultiplyLowWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private GekkoInstruction ParseInstructionNOP(string line, int instructionPointer)
        {
            return new NoOperationInstruction(instructionPointer);
        }

        private GekkoInstruction ParseInstructionORI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "ori");
            var ra = ParseRegister(parameters[0]);
            var rs = ParseRegister(parameters[1]);
            var uimm = ParseIntegerLiteral(parameters[2]);
            return new OrImmediateInstruction(instructionPointer, ra, rs, uimm);
        }

        private GekkoInstruction ParseInstructionSTW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "stw");
            var rs = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new StoreWordInstruction(instructionPointer, rs, offset, ra);
        }

        private GekkoInstruction ParseInstructionSTWU(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "stwu");
            var rs = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new StoreWordWithUpdateInstruction(instructionPointer, rs, offset, ra);
        }

        private GekkoInstruction ParseInstructionSUB(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "sub");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new SubtractFromInstruction(instructionPointer, rd, rb, ra, false, false);
        }

        private GekkoInstruction ParseInstructionSUBF(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "subf");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new SubtractFromInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        #endregion

        private int ParseRegister(string register)
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

            return ParseIntegerLiteral(literal);
        }

        private int ParseConditionRegister(string register)
        {
            return ParseIntegerLiteral(register.Substring(3));
        }

        private string[] ParseParameters(string line, string op)
        {
            return line.Substring(op.Length).Replace(" ", "").Replace("\t", "").Replace(")","").Split(',', '(');
        }

        private int ParseInstructionPointerLabel(string line)
        {
            var address = line.Remove(line.LastIndexOf(':'));
            return ParseIntegerLiteral(address);
        }

        private long ParseInteger64Literal(string literal)
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

        private int ParseIntegerLiteral(string literal)
        {
            return (int)ParseInteger64Literal(literal);
        }

        private double ParseFloatLiteral(string literal)
        {
            return double.Parse(literal, CultureInfo.InvariantCulture);
        }
    }
}
