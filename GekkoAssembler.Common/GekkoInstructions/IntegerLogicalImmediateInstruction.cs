namespace GekkoAssembler.GekkoInstructions
{
    public sealed class IntegerLogicalImmediateInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            ANDI  = 28,
            ANDIS = 29,
            ORI   = 24,
            ORIS  = 25,
            XORI  = 26,
            XORIS = 27
        }

        public override int ByteCode { get; }

        public IntegerLogicalImmediateInstruction(int address, int rA, int rS, int uimm, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | rS << 21 | rA << 16 | uimm & 0xFFFF);
        }
    }
}
