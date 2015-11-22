namespace GekkoAssembler.GekkoInstructions
{
    public sealed class PairedSingleOneOperandInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            PS_ABS    = 264,
            PS_MR     = 72,
            PS_NABS   = 136,
            PS_NEG    = 40,
            PS_RES    = 24,
            PS_RSQRTE = 26
        }

        public override int ByteCode { get; }

        public PairedSingleOneOperandInstruction(int address, int frD, int frB, bool rc, Opcode opcode) : base(address)
        {
            ByteCode = (4 << 26 | frD << 21 | frB << 11 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
