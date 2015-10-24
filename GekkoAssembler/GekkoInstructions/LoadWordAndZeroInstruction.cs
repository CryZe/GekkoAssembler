namespace GekkoAssembler
{
    public class LoadWordAndZeroInstruction : GekkoInstruction
    {
        public override int Address { get; }
        public override int ByteCode
            => (((((32 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | Offset;

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int Offset { get; }

        public LoadWordAndZeroInstruction(int address, int registerDestination, int registerA, int offset)
        {
            Address = address;
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            Offset = offset;
        }
    }
}
