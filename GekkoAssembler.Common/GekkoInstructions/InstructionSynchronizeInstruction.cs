namespace GekkoAssembler.GekkoInstructions
{
    public sealed class InstructionSynchronizeInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public InstructionSynchronizeInstruction(int address) : base(address)
        {
            ByteCode = (19 << 26 | 150 << 1);
        }
    }
}
