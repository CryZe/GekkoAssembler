namespace GekkoAssembler.GekkoInstructions
{
    public sealed class EquivalentInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public EquivalentInstruction(int address, int rA, int rS, int rB, bool rc) : base(address)
        {
            ByteCode = (31 << 26 | rS << 21 | rA << 16 | rB << 11 | 284 << 1 | (rc ? 1 : 0));
        }
    }
}
