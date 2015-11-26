namespace GekkoAssembler.GekkoInstructions
{
    public sealed class StoreFloatingPointInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            STFD  = 54,
            STFDU = 55,
            STFS  = 52,
            STFSU = 53
        }

        public override int ByteCode { get; }

        public StoreFloatingPointInstruction(int address, int frS, int offset, int rA, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | frS << 21 | rA << 16 | offset & 0xFFFF);
        }
    }
}
