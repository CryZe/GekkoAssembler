namespace GekkoAssembler.GekkoInstructions
{
    public sealed class MoveFromSegmentRegisterIndirectInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public MoveFromSegmentRegisterIndirectInstruction(int address, int rD, int rA) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | rA << 11 | 659 << 1);
        }
    }
}
