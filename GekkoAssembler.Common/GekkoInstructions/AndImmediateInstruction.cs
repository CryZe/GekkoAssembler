namespace GekkoAssembler.GekkoInstructions
{
    public sealed class AndImmediateInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public AndImmediateInstruction(int address, int rA, int rS, int uimm, bool shifted) : base(address)
        {
            int opcode = shifted ? 29 : 28;

            ByteCode = (opcode << 26 | rS << 21 | rA << 16 | uimm & 0xFFFF);
        }
    }
}
