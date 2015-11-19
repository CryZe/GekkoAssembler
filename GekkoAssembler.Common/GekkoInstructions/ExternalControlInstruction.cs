namespace GekkoAssembler.GekkoInstructions
{
    public sealed class ExternalControlInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            ECIWX = 310,
            ECOWX = 438
        }

        public override int ByteCode { get; }

        public ExternalControlInstruction(int address, int rD, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rD << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
