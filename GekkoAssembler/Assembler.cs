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
            return assembleAllLines(new Queue<string>(lines), ref instructionPointer);
        }

        private string dequeueNextLine(Queue<string> lines)
        {
            string line = null;
            while (lines.Any() 
                && string.IsNullOrWhiteSpace(line = reduceLineToCode(lines.Dequeue())))
            { }

            return line;
        }

        private IRCodeBlock assembleAllLines(Queue<string> lines, ref int instructionPointer)
        {
            var units = new List<IIRUnit>();
            string line;

            while ((line = dequeueNextLine(lines)) != null)
            {
                if (line.EndsWith(":"))
                {
                    instructionPointer = ParseInstructionPointerLabel(line);
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

        private GekkoDataSection ParseDataSection(string line, int instructionPointer)
        {
            if (line.StartsWith("str "))
                return ParseStringDataSection(line, instructionPointer);
            if (line.StartsWith("u8"))
                return ParseUnsigned8DataSection(line, instructionPointer);
            if (line.StartsWith("u16"))
                return ParseUnsigned16DataSection(line, instructionPointer);
            if (line.StartsWith("u32"))
                return ParseUnsigned32DataSection(line, instructionPointer);
            if (line.StartsWith("u64"))
                return ParseUnsigned64DataSection(line, instructionPointer);
            if (line.StartsWith("s8"))
                return ParseSigned8DataSection(line, instructionPointer);
            if (line.StartsWith("s16"))
                return ParseSigned16DataSection(line, instructionPointer);
            if (line.StartsWith("s32"))
                return ParseSigned32DataSection(line, instructionPointer);
            if (line.StartsWith("s64"))
                return ParseSigned64DataSection(line, instructionPointer);
            if (line.StartsWith("f32"))
                return ParseFloat32DataSection(line, instructionPointer);
            if (line.StartsWith("f64"))
                return ParseFloat64DataSection(line, instructionPointer);

            return new Unsigned32DataSection(instructionPointer, 0xFFFFFFFF);
        }
        
        private GekkoDataSection ParseStringDataSection(string line, int instructionPointer)
        {
            var literal = line.Substring(line.IndexOf("\""));
            var text = ParseStringLiteral(literal);
            return new StringDataSection(instructionPointer, text);
        }

        private string ParseStringLiteral(string literal)
        {
            return literal.Substring(1, literal.Length - 2);
        }
        
        private GekkoDataSection ParseUnsigned8DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u8");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            return new Unsigned8DataSection(instructionPointer, value);
        }
        
        private GekkoDataSection ParseUnsigned16DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u16");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            return new Unsigned16DataSection(instructionPointer, value);
        }
        
        private GekkoDataSection ParseUnsigned32DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u32");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            return new Unsigned32DataSection(instructionPointer, value);
        }

        private GekkoDataSection ParseUnsigned64DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u64");
            var value = (ulong)ParseInteger64Literal(parameters[0]);
            return new Unsigned64DataSection(instructionPointer, value);
        }

        private GekkoDataSection ParseSigned8DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s8");
            var value = (sbyte)ParseIntegerLiteral(parameters[0]);
            return new Signed8DataSection(instructionPointer, value);
        }

        private GekkoDataSection ParseSigned16DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s16");
            var value = (short)ParseIntegerLiteral(parameters[0]);
            return new Signed16DataSection(instructionPointer, value);
        }

        private GekkoDataSection ParseSigned32DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s32");
            var value = ParseIntegerLiteral(parameters[0]);
            return new Signed32DataSection(instructionPointer, value);
        }

        private GekkoDataSection ParseSigned64DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "s64");
            var value = ParseInteger64Literal(parameters[0]);
            return new Signed64DataSection(instructionPointer, value);
        }

        private GekkoDataSection ParseFloat32DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "f32");
            var value = (float)ParseFloatLiteral(parameters[0]);
            return new Float32DataSection(instructionPointer, value);
        }

        private GekkoDataSection ParseFloat64DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "f64");
            var value = ParseFloatLiteral(parameters[0]);
            return new Float64DataSection(instructionPointer, value);
        }

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
            return new NoOperationInstruction(instructionPointer);
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

        private GekkoInstruction ParseInstructionNOP(string line, int instructionPointer)
        {
            return new NoOperationInstruction(instructionPointer);
        }

        private GekkoInstruction ParseInstructionBLR(string line, int instructionPointer)
        {
            return new BranchToLinkRegisterInstruction(instructionPointer);
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
