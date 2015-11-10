namespace GekkoAssembler.GekkoInstructions
{
    public sealed class InstructionSynchronizeInstruction : GekkoInstruction
    {
        public override int Address  { get; }
        public override int ByteCode { get; }

        public InstructionSynchronizeInstruction(int address)
        {
            this.Address  = address;
            this.ByteCode = (19 << 26 | 150 << 1);
        }
    }
}
