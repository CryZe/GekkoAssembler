namespace GekkoAssembler.GekkoInstructions
{
    public sealed class MoveFromTimeBaseInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public MoveFromTimeBaseInstruction(int address, int rD, int timebase) : base(address)
        {
            int swappedTimeBase = ((timebase << 5) & 0x3E0) | timebase >> 5;
            ByteCode = (31 << 26 | rD << 21 | swappedTimeBase << 11 | 371 << 1);
        }
    }
}
