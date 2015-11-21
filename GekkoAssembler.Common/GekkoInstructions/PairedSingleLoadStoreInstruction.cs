namespace GekkoAssembler.GekkoInstructions
{
    public sealed class PairedSingleLoadStoreInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            PSQ_L   = 56,
            PSQ_LU  = 57,
            PSQ_ST  = 60,
            PSQ_STU = 61
        }

        public override int ByteCode { get; }

        public PairedSingleLoadStoreInstruction(int address, int frS, int offset, int rA, bool w, int i, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | frS << 21 | rA << 16 | (w ? 1 : 0) << 15 | i << 12 | offset & 0xFFF);
        }
    }
}
