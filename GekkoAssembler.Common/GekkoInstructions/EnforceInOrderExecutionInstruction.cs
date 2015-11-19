namespace GekkoAssembler.GekkoInstructions
{
    public sealed class EnforceInOrderExecutionInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public EnforceInOrderExecutionInstruction(int address) : base(address)
        {
            ByteCode = (31 << 26 | 854 << 1);
        }
    }
}
