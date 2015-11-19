namespace GekkoAssembler.GekkoInstructions
{
    public sealed class MoveFromConditionRegisterInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public MoveFromConditionRegisterInstruction(int address, int rD) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | 19 << 1);
        }
    }
}
