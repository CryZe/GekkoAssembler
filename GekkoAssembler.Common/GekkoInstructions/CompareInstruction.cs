namespace GekkoAssembler.GekkoInstructions
{
    public sealed class CompareInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public CompareInstruction(int address, int crfD, int L, int rA, int rB, bool logical) : base(address)
        {
            var secondary_opcode = logical ? 32 : 0;

            ByteCode = (31 << 26 | crfD << 23 | L << 21 | rA << 16 | rB << 11 | secondary_opcode << 1);
        }
    }
}
