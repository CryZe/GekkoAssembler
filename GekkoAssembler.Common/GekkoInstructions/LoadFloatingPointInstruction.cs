namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadFloatingPointInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LFD  = 50,
            LFDU = 51,
            LFS  = 48,
            LFSU = 49
        }

        public override int ByteCode { get; }

        public LoadFloatingPointInstruction(int address, int frD, int offset, int rA, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | frD << 21 | rA << 16 | offset & 0xFFFF);
        }
    }
}
