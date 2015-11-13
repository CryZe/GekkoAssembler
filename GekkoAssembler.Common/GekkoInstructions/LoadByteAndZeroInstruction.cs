namespace GekkoAssembler.GekkoInstructions
{
    public class LoadByteAndZeroInstruction : GekkoInstruction
    {
        public override int ByteCode
            => (((((34 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | Offset;

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int Offset { get; }

        public LoadByteAndZeroInstruction(int address, int registerDestination, int registerA, int offset) : base(address)
        {
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            Offset = offset;
        }
    }
}
