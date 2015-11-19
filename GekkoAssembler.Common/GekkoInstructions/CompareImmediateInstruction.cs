namespace GekkoAssembler.GekkoInstructions
{
    public sealed class CompareImmediateInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public CompareImmediateInstruction(int address, int crfD, int L, int rA, int simm, bool logical) : base(address)
        {
            var opcode = logical ? 10 : 11;

            ByteCode = (opcode << 26 | crfD << 23 | L << 21 | rA << 16 | simm & 0xFFFF);
        }
    }
}
