namespace GekkoAssembler.GekkoInstructions
{
    public sealed class DivideWordInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public DivideWordInstruction(int address, int rD, int rA, int rB, bool oe, bool rc, bool unsigned) : base(address)
        {
            int opcode = unsigned ? 459 : 491;

            ByteCode = (31 << 26 | rD << 21 | rA << 16 | rB << 11 | (oe ? 1 : 0) << 10 | opcode << 1 | (rc ? 1 : 0));
        }
    }
}
