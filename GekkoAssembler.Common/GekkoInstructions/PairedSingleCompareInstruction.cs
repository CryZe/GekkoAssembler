namespace GekkoAssembler.GekkoInstructions
{
    public sealed class PairedSingleCompareInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            PS_CMPO0 = 32,
            PS_CMPO1 = 96,
            PS_CMPU0 = 0,
            PS_CMPU1 = 64
        }

        public override int ByteCode { get; }

        public PairedSingleCompareInstruction(int address, int crfD, int frA, int frB, Opcode opcode) : base(address)
        {
            ByteCode = (4 << 26 | crfD << 23 | frA << 16 | frB << 11 | (int)opcode << 1);
        }
    }
}
