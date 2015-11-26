namespace GekkoAssembler.GekkoInstructions
{
    public sealed class FloatingPointTwoOperandInstruction : GekkoInstruction
    {
        public enum Variant
        {
            Single = 59,
            Double = 63
        }

        public enum Opcode
        {
            FADD = 21,
            FDIV = 18,
            FSUB = 20
        }

        public override int ByteCode { get; }

        public FloatingPointTwoOperandInstruction(int address, int frD, int frA, int frB, bool rc, Variant variant, Opcode opcode) : base(address)
        {
            ByteCode = ((int)variant << 26 | frD << 21 | frA << 16 | frB << 11 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
