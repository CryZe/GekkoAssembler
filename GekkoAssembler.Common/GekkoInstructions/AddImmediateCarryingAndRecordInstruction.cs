namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AddImmediateCarryingAndRecordInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public AddImmediateCarryingAndRecordInstruction(int address, int rD, int rA, int simm) : base(address)
        {
            ByteCode = (13 << 26 | rD << 21 | rA << 16 | simm & 0xFFFF);
        }
    }
}
