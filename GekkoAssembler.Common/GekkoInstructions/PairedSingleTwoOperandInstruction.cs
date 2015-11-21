namespace GekkoAssembler.GekkoInstructions
{
    public sealed class PairedSingleTwoOperandInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            PS_ADD     = 21,
            PS_DIV     = 18,
            PS_MERGE00 = 528,
            PS_MERGE01 = 560,
            PS_MERGE10 = 592,
            PS_MERGE11 = 624,
            PS_SUB     = 20
        }

        public override int ByteCode { get; }

        public PairedSingleTwoOperandInstruction(int address, int frD, int frA, int frB, bool rc, Opcode opcode) : base(address)
        {
            ByteCode = (4 << 26 | frD << 21 | frA << 16 | frB << 11 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
