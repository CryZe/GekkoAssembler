namespace GekkoAssembler.GekkoInstructions
{
    public sealed class SignExtensionInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            Byte     = 954,
            Halfword = 922
        }

        public override int ByteCode { get; }

        public SignExtensionInstruction(int address, int rA, int rS, bool rc, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rS << 21 | rA << 16 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
