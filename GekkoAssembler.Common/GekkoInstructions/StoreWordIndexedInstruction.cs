namespace GekkoAssembler.GekkoInstructions
{
    public sealed class StoreWordIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            STWBRX = 662,
            STWCX  = 150,
            STWUX  = 183,
            STWX   = 151
        }

        public override int ByteCode { get; }

        public StoreWordIndexedInstruction(int address, int rS, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rS << 21 | rA << 16 | rB << 11 | (int)opcode << 1);

            if (opcode == Opcode.STWCX)
                ByteCode |= 1;
        }
    }
}
