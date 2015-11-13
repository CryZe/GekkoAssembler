namespace GekkoAssembler.GekkoInstructions
{
    public class LoadHalfWordAndZeroInstruction : GekkoInstruction
    {
        public override int ByteCode
            => (((((40 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | Offset;

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int Offset { get; }

        public LoadHalfWordAndZeroInstruction(int address, int registerDestination, int registerA, int offset) : base(address)
        {
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            Offset = offset;
        }
    }
}
