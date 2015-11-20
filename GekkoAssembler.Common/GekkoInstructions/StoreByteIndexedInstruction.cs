namespace GekkoAssembler.GekkoInstructions
{
    public sealed class StoreByteIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            STBX  = 215,
            STBUX = 247
        }

        public override int ByteCode { get; }

        public StoreByteIndexedInstruction(int address, int rS, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rS << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
