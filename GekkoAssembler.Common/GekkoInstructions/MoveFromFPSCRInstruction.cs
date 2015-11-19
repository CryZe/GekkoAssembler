namespace GekkoAssembler.GekkoInstructions
{
    public sealed class MoveFromFPSCRInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public MoveFromFPSCRInstruction(int address, int frD, bool rc) : base(address)
        {
            ByteCode = (63 << 26 | frD << 21 | 583 << 1 | (rc ? 1 : 0));
        }
    }
}
