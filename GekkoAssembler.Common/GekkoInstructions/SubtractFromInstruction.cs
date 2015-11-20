namespace GekkoAssembler.GekkoInstructions
{
    public sealed class SubtractFromInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public SubtractFromInstruction(int address, int rD, int rA, int rB, bool oe, bool rc) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | rA << 16 | rB << 11 | (oe ? 1 : 0) << 10 | 40 << 1 | (rc ? 1 : 0));
        }
    }
}
