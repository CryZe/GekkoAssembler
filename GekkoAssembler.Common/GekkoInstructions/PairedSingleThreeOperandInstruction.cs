namespace GekkoAssembler.GekkoInstructions
{
    public sealed class PairedSingleThreeOperandInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            PS_MADD   = 29,
            PS_MADDS0 = 14,
            PS_MADDS1 = 15,
            PS_MSUB   = 28,
            PS_NMADD  = 31,
            PS_NMSUB  = 30,
            PS_SEL    = 23,
            PS_SUM0   = 10,
            PS_SUM1   = 11
        }

        public override int ByteCode { get; }

        public PairedSingleThreeOperandInstruction(int address, int frD, int frA, int frC, int frB, bool rc, Opcode opcode) : base(address)
        {
            ByteCode = (4 << 26 | frD << 21 | frA << 16 | frB << 11 | frC << 6 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
