namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadByteInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LBZ  = 34,
            LBZU = 35
        }

        public override int ByteCode { get; }

        public LoadByteInstruction(int address, int rD, int offset, int rA, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | rD << 21 | rA << 16 | offset & 0xFFFF);
        }
    }
}
