namespace GekkoAssembler.GekkoInstructions
{
    public sealed class MoveFromMSRInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public MoveFromMSRInstruction(int address, int rD) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | 83 << 1);
        }
    }
}
