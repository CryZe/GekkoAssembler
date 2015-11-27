namespace GekkoAssembler.GekkoInstructions
{
    public sealed class IntegerLogicalInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            AND  = 28,
            ANDC = 60,
            EQV  = 284,
            NAND = 476,
            NOR  = 124,
            OR   = 444,
            ORC  = 412,
            XOR  = 316
        }

        public override int ByteCode { get; }

        public IntegerLogicalInstruction(int address, int rA, int rS, int rB, bool rc, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rS << 21 | rA << 16 | rB << 11 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
