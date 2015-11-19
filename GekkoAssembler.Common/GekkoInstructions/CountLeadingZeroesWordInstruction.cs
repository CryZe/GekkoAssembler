namespace GekkoAssembler.GekkoInstructions
{
    public sealed class CountLeadingZeroesWordInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public CountLeadingZeroesWordInstruction(int address, int rA, int rS, bool rc) : base(address)
        {
            ByteCode = (31 << 26 | rS << 21 | rA << 16 | 26 << 1 | (rc ? 1 : 0));
        }
    }
}
