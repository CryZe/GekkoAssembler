namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AndInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public AndInstruction(int address, int rA, int rS, int rB, bool rc, bool complement) : base(address)
        {
            int opcode = complement ? 60 : 28;

            ByteCode = (31 << 26 | rS << 21 | rA << 16 | rB << 11 | opcode << 1 | (rc ? 1 : 0));
        }
    }
}
