namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AddCarryingInstruction : GekkoInstruction
    {
        public override int Address  { get; }
        public override int ByteCode { get; }

        public AddCarryingInstruction(int address, int rD, int rA, int rB, bool oe, bool rc)
        {
            this.Address = address;
            this.ByteCode = (31 << 26 | rD << 21 | rA << 16 | rB << 11 | (oe ? 1 : 0) << 10 | 10 << 1 | (rc ? 1 : 0));
        }
    }
}
