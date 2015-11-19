namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadHalfWordInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LHZ  = 40,
            LHZU = 41,
            LHA  = 42,
            LHAU = 43
        }

        public override int ByteCode { get; }

        public LoadHalfWordInstruction(int address, int rD, int offset, int rA, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | rD << 21 | rA << 16 | offset & 0xFFFF);
        }
    }
}
