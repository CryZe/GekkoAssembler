namespace GekkoAssembler.GekkoInstructions
{
    public sealed class MoveFromSegmentRegisterInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public MoveFromSegmentRegisterInstruction(int address, int rD, int segmentRegister) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | segmentRegister << 16 | 595 << 1);
        }
    }
}
