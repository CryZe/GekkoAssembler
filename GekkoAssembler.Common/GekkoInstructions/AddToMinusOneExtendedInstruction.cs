namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AddToMinusOneExtendedInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public AddToMinusOneExtendedInstruction(int address, int rD, int rA, bool oe, bool rc) : base(address)
        {
            this.ByteCode = (31 << 26 | rD << 21 | rA << 16 | (oe ? 1 : 0) << 10 | 234 << 1 | (rc ? 1 : 0));
        }
    }
}
