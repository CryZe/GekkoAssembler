namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadHalfWordIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LHAUX = 375,
            LHAX  = 343,
            LHBRX = 790,
            LHZUX = 311,
            LHZX  = 279
        }

        public override int ByteCode { get; }

        public LoadHalfWordIndexedInstruction(int address, int rD, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
