namespace GekkoAssembler.GekkoInstructions
{
    public sealed class FloatingPointThreeOperandInstruction : GekkoInstruction
    {
        public enum Variant
        {
            Single = 59,
            Double = 63
        }

        public enum Opcode
        {
            FMADD  = 29,
            FMSUB  = 28,
            FNMADD = 31,
            FNMSUB = 30,
            FSEL   = 23
        }

        public override int ByteCode { get; }

        public FloatingPointThreeOperandInstruction(int address, int frD, int frA, int frC, int frB, bool rc, Variant variant, Opcode opcode) : base(address)
        {
            ByteCode = ((int)variant << 26 | frD << 21 | frA << 16 | frB << 11 | frC << 6 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
