namespace GekkoAssembler.GekkoInstructions
{
    public sealed class InstructionCacheBlockInvalidateInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public InstructionCacheBlockInvalidateInstruction(int address, int rA, int rB) : base(address)
        {
            ByteCode = (31 << 26 | rA << 16 | rB << 11 | 982 << 1);
        }
    }
}
