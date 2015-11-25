namespace GekkoAssembler.GekkoInstructions
{
    public sealed class FloatingPointCompareInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            FCMPO = 32,
            FCMPU = 0
        }

        public override int ByteCode { get; }

        public FloatingPointCompareInstruction(int address, int crfD, int frA, int frB, Opcode opcode) : base(address)
        {
            ByteCode = (63 << 26 | crfD << 23 | frA << 16 | frB << 11 | (int)opcode << 1);
        }
    }
}
