namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadWordInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LWZ  = 32,
            LWZU = 33
        }

        public override int ByteCode { get; }

        public LoadWordInstruction(int address, int rD, int offset, int rA, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | rD << 21 | rA << 16 | offset & 0xFFFF);
        }
    }
}
