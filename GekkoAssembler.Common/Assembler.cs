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
            Optimizers = new List<IOptimizer>() { new WriteDataOptimizer() };
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
            return assembleAllLines(new Queue<string>(lines), instructionPointer);
        }

        private string dequeueNextLine(Queue<string> lines)
        {
            string line = null;
            while (lines.Any()
                && string.IsNullOrWhiteSpace(line = reduceLineToCode(lines.Dequeue())))
            { }

            return line;
        }

        private IRCodeBlock assembleAllLines(Queue<string> lines, int instructionPointer)
        {
            var units = new List<IIRUnit>();
            string line;

            while (!string.IsNullOrWhiteSpace(line = dequeueNextLine(lines)))
            {
                if (line.EndsWith(":"))
                {
                    instructionPointer = ParseInstructionPointerLabel(line);
                }
                else if (line.StartsWith("!"))
                {
                    if (line == "!end")
                        break;

                    var specialInstruction = ParseSpecialInstruction(line.Substring(1), instructionPointer, lines);
                    units.Add(specialInstruction);
                }
                else if (line.StartsWith("."))
                {
                    var dataSection = ParseDataSection(line.Substring(1), instructionPointer);
                    units.Add(dataSection);
                    instructionPointer += dataSection.Data.Length;
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

            var block = new IRCodeBlock(units);

            foreach (var optimizer in Optimizers)
            {
                block = optimizer.Optimize(block);
            }

            return block;
        }

        private IIRUnit ParseSpecialInstruction(string line, int instructionPointer, Queue<string> lines)
        {
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
                return ParseUnsigned8Add(line, instructionPointer);
            if (line.StartsWith("u16add "))
                return ParseUnsigned16Add(line, instructionPointer);
            if (line.StartsWith("u32add "))
                return ParseUnsigned32Add(line, instructionPointer);
            if (line.StartsWith("s8add "))
                return ParseSigned8Add(line, instructionPointer);
            if (line.StartsWith("s16add "))
                return ParseSigned16Add(line, instructionPointer);
            if (line.StartsWith("s32add "))
                return ParseSigned32Add(line, instructionPointer);
            if (line.StartsWith("f32add "))
                return ParseFloat32Add(line, instructionPointer);

            throw new ArgumentException($"The specified special instruction { line } is not supported.");
        }

        #region Add

        private IIRUnit ParseUnsigned8Add(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u8add");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            return new IRUnsigned8Add(instructionPointer, value);
        }

        private IIRUnit ParseUnsigned16Add(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u16add");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            return new IRUnsigned16Add(instructionPointer, value);
        }

        private IIRUnit ParseUnsigned32Add(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u32add");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            return new IRUnsigned32Add(instructionPointer, value);
        }

        private IIRUnit ParseSigned8Add(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s8add");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            return new IRSigned8Add(instructionPointer, value);
        }

        private IIRUnit ParseSigned16Add(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s16add");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            return new IRSigned16Add(instructionPointer, value);
        }

        private IIRUnit ParseSigned32Add(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s32add");
            var value = ParseIntegerLiteral(parameters[0]);
            return new IRSigned32Add(instructionPointer, value);
        }

        private IIRUnit ParseFloat32Add(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "f32add");
            var value = (float)ParseFloatLiteral(parameters[0]);
            return new IRFloat32Add(instructionPointer, value);
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
            if (line.StartsWith("blr"))
                return ParseInstructionBLR(line, instructionPointer);
            if (line.StartsWith("b "))
                return ParseInstructionB(line, instructionPointer);
            if (line.StartsWith("bl "))
                return ParseInstructionBL(line, instructionPointer);
            if (line.StartsWith("ba "))
                return ParseInstructionBA(line, instructionPointer);
            if (line.StartsWith("bla "))
                return ParseInstructionBLA(line, instructionPointer);
            if (line.StartsWith("addi "))
                return ParseInstructionADDI(line, instructionPointer);
            if (line.StartsWith("addis "))
                return ParseInstructionADDIS(line, instructionPointer);
            if (line.StartsWith("mulli "))
                return ParseInstructionMULLI(line, instructionPointer);
            if (line.StartsWith("stw "))
                return ParseInstructionSTW(line, instructionPointer);
            if (line.StartsWith("stwu "))
                return ParseInstructionSTWU(line, instructionPointer);
            if (line.StartsWith("lwz "))
                return ParseInstructionLWZ(line, instructionPointer);
            if (line.StartsWith("lhz "))
                return ParseInstructionLHZ(line, instructionPointer);
            if (line.StartsWith("lbz "))
                return ParseInstructionLBZ(line, instructionPointer);
            if (line.StartsWith("lis "))
                return ParseInstructionLIS(line, instructionPointer);
            if (line.StartsWith("ori "))
                return ParseInstructionORI(line, instructionPointer);
            if (line.StartsWith("crclr "))
                return ParseInstructionCRCLR(line, instructionPointer);
            if (line.StartsWith("crxor "))
                return ParseInstructionCRXOR(line, instructionPointer);
            if (line.StartsWith("sub "))
                return ParseInstructionSUB(line, instructionPointer);
            if (line.StartsWith("subf "))
                return ParseInstructionSUBF(line, instructionPointer);
            if (line.StartsWith("mullw "))
                return ParseInstructionMULLW(line, instructionPointer);
            if (line.StartsWith("divw "))
                return ParseInstructionDIVW(line, instructionPointer);
            if (line.StartsWith("mflr "))
                return ParseInstructionMFLR(line, instructionPointer);
            if (line.StartsWith("mfspr "))
                return ParseInstructionMFSPR(line, instructionPointer);
            if (line.StartsWith("mtlr "))
                return ParseInstructionMTLR(line, instructionPointer);
            if (line.StartsWith("mtspr "))
                return ParseInstructionMTSPR(line, instructionPointer);
            if (line.StartsWith("nop"))
                return ParseInstructionNOP(line, instructionPointer);

            throw new ArgumentException($"The specified instruction { line } is not supported.");
        }

        #region Gekko Instructions

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

        private GekkoInstruction ParseInstructionMULLW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mullw");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new MultiplyLowWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private GekkoInstruction ParseInstructionDIVW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "divw");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new DivideWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private GekkoInstruction ParseInstructionSTWU(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "stwu");
            var rs = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new StoreWordWithUpdateInstruction(instructionPointer, rs, offset, ra);
        }

        private GekkoInstruction ParseInstructionSTW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "stw");
            var rs = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new StoreWordInstruction(instructionPointer, rs, offset, ra);
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

        private GekkoInstruction ParseInstructionLWZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lwz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private GekkoInstruction ParseInstructionLHZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lhz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadHalfWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private GekkoInstruction ParseInstructionLBZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lbz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadByteAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private GekkoInstruction ParseInstructionBLA(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "bla");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, true, true);
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

        private GekkoInstruction ParseInstructionB(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "b");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, false, false);
        }

        private GekkoInstruction ParseInstructionCRCLR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crclr");
            var crbd = ParseConditionRegister(parameters[0]);
            return new ConditionRegisterClearInstruction(instructionPointer, crbd);
        }

        private GekkoInstruction ParseInstructionCRXOR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crxor");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterXORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private GekkoInstruction ParseInstructionMULLI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mulli");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new MultiplyLowImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private GekkoInstruction ParseInstructionADDI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "addi");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new AddImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private GekkoInstruction ParseInstructionLIS(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lis");
            var rd = ParseRegister(parameters[0]);
            var simm = ParseIntegerLiteral(parameters[1]);
            return new LoadImmediateShiftedInstruction(instructionPointer, rd, simm);
        }

        private GekkoInstruction ParseInstructionADDIS(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "addis");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new AddImmediateShiftedInstruction(instructionPointer, rd, ra, simm);
        }

        private GekkoInstruction ParseInstructionORI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "ori");
            var ra = ParseRegister(parameters[0]);
            var rs = ParseRegister(parameters[1]);
            var uimm = ParseIntegerLiteral(parameters[2]);
            return new OrImmediateInstruction(instructionPointer, ra, rs, uimm);
        }

        private GekkoInstruction ParseInstructionNOP(string line, int instructionPointer)
        {
            return new NoOperationInstruction(instructionPointer);
        }

        private GekkoInstruction ParseInstructionBLR(string line, int instructionPointer)
        {
            return new BranchToLinkRegisterInstruction(instructionPointer);
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
