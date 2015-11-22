namespace GekkoAssembler.GekkoInstructions
{
    public sealed class PairedSingleMultiplyInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            PS_MUL   = 25,
            PS_MULS0 = 12,
            PS_MULS1 = 13
        }

        public override int ByteCode { get; }

        public PairedSingleMultiplyInstruction(int address, int frD, int frA, int frC, bool rc, Opcode opcode) : base(address)
        {
            ByteCode = (4 << 26 | frD << 21 | frA << 16 | frC << 6 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
