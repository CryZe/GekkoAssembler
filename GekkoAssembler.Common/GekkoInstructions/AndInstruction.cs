namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AndInstruction : GekkoInstruction
    {
        public override int Address  { get; }
        public override int ByteCode { get; }

        public AndInstruction(int address, int rA, int rS, int rB, bool rc)
        {
            this.Address = address;
            this.ByteCode = (31 << 26 | rS << 21 | rA << 16 | rB << 11 | 28 << 1 | (rc ? 1 : 0));
        }
    }
}
