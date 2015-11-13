namespace GekkoAssembler.GekkoInstructions
{
    public class LoadFloatingPointSingleInstruction : GekkoInstruction
    {
        public override int ByteCode
            => (((((48 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | Offset;

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int Offset { get; }

        public LoadFloatingPointSingleInstruction(int address, int registerDestination, int registerA, int offset) : base(address)
        {
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            Offset = offset;
        }
    }
}
