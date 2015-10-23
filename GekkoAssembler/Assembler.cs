using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GekkoAssembler
{
    public class Assembler
    {
        private static string reduceLineToCode(string line)
        {
            if (line.Contains("//"))
                line = line.Substring(0, line.IndexOf("//"));
            
            return line.Trim();
        }
        
        public GekkoAssembly AssembleAllLines(IEnumerable<string> lines)
        {
            var gekkoAssembly = new GekkoAssembly();
            var instructionPointer = 0x00000000;
            foreach (var line in lines
                .Select(line => reduceLineToCode(line))
                .Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                if (line.EndsWith(":"))
                {
                    instructionPointer = ParseInstructionPointerLabel(line);
                }
                else if (line.StartsWith("."))
                {
                    var dataSection = ParseDataSection(line.Substring(1), instructionPointer);
                    gekkoAssembly.Add(dataSection);
                    instructionPointer += dataSection.Data.Length;
                }
                else
                {
                    //Align the instruction
                    instructionPointer = (instructionPointer + 3) & ~3;
                    var instruction = ParseInstruction(line, instructionPointer);
                    gekkoAssembly.Add(instruction);
                    instructionPointer += 4;
                }
            }
            return gekkoAssembly;
        }

        private IGekkoDataSection ParseDataSection(string line, int instructionPointer)
        {
            if (line.StartsWith("str "))
                return ParseStringDataSection(line, instructionPointer);
            if (line.StartsWith("byte"))
                return ParseByteDataSection(line, instructionPointer);
            if (line.StartsWith("u16"))
                return ParseUnsigned16DataSection(line, instructionPointer);
            if (line.StartsWith("u32"))
                return ParseUnsigned32DataSection(line, instructionPointer);
                
            return new Unsigned32DataSection(instructionPointer, 0xFFFFFFFF);
        }
        
        private IGekkoDataSection ParseStringDataSection(string line, int instructionPointer)
        {
            var literal = line.Substring(line.IndexOf("\""));
            var text = ParseStringLiteral(literal);
            return new StringDataSection(instructionPointer, text);
        }

        private string ParseStringLiteral(string literal)
        {
            return literal.Substring(1, literal.Length - 2);
        }
        
        private IGekkoDataSection ParseByteDataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "byte");
            var value = (byte)ParseIntegerLiteral(parameters[0]);
            return new ByteDataSection(instructionPointer, value);
        }
        
        private IGekkoDataSection ParseUnsigned16DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u16");
            var value = (ushort)ParseIntegerLiteral(parameters[0]);
            return new Unsigned16DataSection(instructionPointer, value);
        }
        
        private IGekkoDataSection ParseUnsigned32DataSection(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "u32");
            var value = (uint)ParseIntegerLiteral(parameters[0]);
            return new Unsigned32DataSection(instructionPointer, value);
        }

        private IGekkoInstruction ParseInstruction(string line, int instructionPointer)
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

        private IGekkoInstruction ParseInstructionSUB(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "sub");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new SubtractFromInstruction(instructionPointer, rd, rb, ra, false, false);
        }

        private IGekkoInstruction ParseInstructionSUBF(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "subf");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new SubtractFromInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private IGekkoInstruction ParseInstructionMULLW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mullw");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new MultiplyLowWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private IGekkoInstruction ParseInstructionDIVW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "divw");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var rb = ParseRegister(parameters[2]);
            return new DivideWordInstruction(instructionPointer, rd, ra, rb, false, false);
        }

        private IGekkoInstruction ParseInstructionSTWU(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "stwu");
            var rs = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new StoreWordWithUpdateInstruction(instructionPointer, rs, offset, ra);
        }

        private IGekkoInstruction ParseInstructionSTW(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "stw");
            var rs = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new StoreWordInstruction(instructionPointer, rs, offset, ra);
        }

        private IGekkoInstruction ParseInstructionMFLR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mflr");
            var rd = ParseRegister(parameters[0]);
            return new MoveFromLinkRegisterInstruction(instructionPointer, rd);
        }

        private IGekkoInstruction ParseInstructionMFSPR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mfspr");
            var rd = ParseRegister(parameters[0]);
            var spr = ParseIntegerLiteral(parameters[1]);
            return new MoveFromSpecialPurposeRegisterInstruction(instructionPointer, rd, spr);
        }

        private IGekkoInstruction ParseInstructionMTLR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mtlr");
            var rs = ParseRegister(parameters[0]);
            return new MoveToLinkRegisterInstruction(instructionPointer, rs);
        }

        private IGekkoInstruction ParseInstructionMTSPR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mtspr");
            var spr = ParseIntegerLiteral(parameters[0]);
            var rs = ParseRegister(parameters[1]);
            return new MoveToSpecialPurposeRegisterInstruction(instructionPointer, spr, rs);
        }

        private IGekkoInstruction ParseInstructionLWZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lwz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private IGekkoInstruction ParseInstructionLHZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lhz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadHalfWordAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private IGekkoInstruction ParseInstructionLBZ(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lbz");
            var rd = ParseRegister(parameters[0]);
            var offset = ParseIntegerLiteral(parameters[1]);
            var ra = ParseRegister(parameters[2]);
            return new LoadByteAndZeroInstruction(instructionPointer, rd, ra, offset);
        }

        private IGekkoInstruction ParseInstructionBLA(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "bla");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, true, true);
        }

        private IGekkoInstruction ParseInstructionBA(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "ba");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, true, false);
        }

        private IGekkoInstruction ParseInstructionBL(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "bl");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, false, true);
        }

        private IGekkoInstruction ParseInstructionB(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "b");
            var targetAddress = ParseIntegerLiteral(parameters[0]);
            return new BranchInstruction(instructionPointer, targetAddress, false, false);
        }

        private IGekkoInstruction ParseInstructionCRCLR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crclr");
            var crbd = ParseConditionRegister(parameters[0]);
            return new ConditionRegisterClearInstruction(instructionPointer, crbd);
        }

        private IGekkoInstruction ParseInstructionCRXOR(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "crxor");
            var crbd = ParseConditionRegister(parameters[0]);
            var crba = ParseConditionRegister(parameters[1]);
            var crbb = ParseConditionRegister(parameters[2]);
            return new ConditionRegisterXORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private IGekkoInstruction ParseInstructionMULLI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "mulli");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new MultiplyLowImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private IGekkoInstruction ParseInstructionADDI(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "addi");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new AddImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private IGekkoInstruction ParseInstructionLIS(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "lis");
            var rd = ParseRegister(parameters[0]);
            var simm = ParseIntegerLiteral(parameters[1]);
            return new LoadImmediateShiftedInstruction(instructionPointer, rd, simm);
        }

        private IGekkoInstruction ParseInstructionADDIS(string line, int instructionPointer)
        {
            var parameters = ParseParameters(line, "addis");
            var rd = ParseRegister(parameters[0]);
            var ra = ParseRegister(parameters[1]);
            var simm = ParseIntegerLiteral(parameters[2]);
            return new AddImmediateShiftedInstruction(instructionPointer, rd, ra, simm);
        }

        private IGekkoInstruction ParseInstructionORI(string line, int instructionPointer)
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

        private IGekkoInstruction ParseInstructionNOP(string line, int instructionPointer)
        {
            return new NoOperationInstruction(instructionPointer);
        }

        private IGekkoInstruction ParseInstructionBLR(string line, int instructionPointer)
        {
            return new BranchToLinkRegisterInstruction(instructionPointer);
        }

        private int ParseInstructionPointerLabel(string line)
        {
            var address = line.Remove(line.LastIndexOf(':'));
            return ParseIntegerLiteral(address);
        }

        private int ParseIntegerLiteral(string literal)
        {
            if (literal.Contains("0x"))
            {
                //Hexadecimal
                literal = literal.Replace("0x", "");
                var negative = literal.StartsWith("-");
                if (negative)
                    literal = literal.Substring(1);
                return (negative ? -1 : 1) * int.Parse(literal, NumberStyles.HexNumber);
            }
            else
            {
                return int.Parse(literal, NumberStyles.Integer);
            }
        }
    }
}
