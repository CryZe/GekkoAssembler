namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AddToZeroExtendedInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public AddToZeroExtendedInstruction(int address, int rD, int rA, bool oe, bool rc) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | rA << 16 | (oe ? 1 : 0) << 10 | 202 << 1 | (rc ? 1 : 0));
        }
    }
}
