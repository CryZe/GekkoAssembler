namespace GekkoAssembler.GekkoInstructions
{
    public sealed class InstructionCacheBlockInvalidateInstruction : GekkoInstruction
    {
        public override int Address  { get; }
        public override int ByteCode { get; }

        public InstructionCacheBlockInvalidateInstruction(int address, int rA, int rB)
        {
            this.Address = address;
            this.ByteCode = (31 << 26 | rA << 16 | rB << 11 | 982 << 1);
        }
    }
}
