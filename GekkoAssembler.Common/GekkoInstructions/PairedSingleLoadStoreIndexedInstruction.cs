namespace GekkoAssembler.GekkoInstructions
{
    public sealed class PairedSingleLoadStoreIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            PSQ_LUX  = 38,
            PSQ_LX   = 6,
            PSQ_STUX = 39,
            PSQ_STX  = 7
        }

        public override int ByteCode { get; }

        public PairedSingleLoadStoreIndexedInstruction(int address, int frS, int rA, int rB, bool w, int i, Opcode opcode) : base(address)
        {
            ByteCode = (4 << 26 | frS << 21 | rA << 16 | rB << 11 | (w ? 1 : 0) << 10 | (i & 0xF) << 7 | (int)opcode << 1);
        }
    }
}
