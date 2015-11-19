namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadWordIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LWARX = 20,
            LWBRX = 534,
            LWZUX = 55,
            LWZX  = 23
        }

        public override int ByteCode { get; }

        public LoadWordIndexedInstruction(int address, int rD, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
