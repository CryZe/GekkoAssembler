namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AddImmediateCarryingInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public AddImmediateCarryingInstruction(int address, int rD, int rA, int simm) : base(address)
        {
            ByteCode = (12 << 26 | rD << 21 | rA << 16 | simm & 0xFFFF);
        }
    }
}
