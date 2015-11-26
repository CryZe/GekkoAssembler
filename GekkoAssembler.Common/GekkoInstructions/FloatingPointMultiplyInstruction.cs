namespace GekkoAssembler.GekkoInstructions
{
    public sealed class FloatingPointMultiplyInstruction : GekkoInstruction
    {
        public enum Variant
        {
            Single = 59,
            Double = 63
        }

        public override int ByteCode { get; }

        public FloatingPointMultiplyInstruction(int address, int frD, int frA, int frC, bool rc, Variant variant) : base(address)
        {
            ByteCode = ((int)variant << 26 | frD << 21 | frA << 16 | frC << 6 | 25 << 1 | (rc ? 1 : 0));
        }
    }
}
