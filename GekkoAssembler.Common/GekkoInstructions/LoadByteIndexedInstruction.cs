namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadByteIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LBZUX = 119,
            LBZX  = 87
        }

        public override int ByteCode { get; }

        public LoadByteIndexedInstruction(int address, int rD, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
