namespace GekkoAssembler
{
    public class LoadHalfWordAndZeroInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((40 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | Offset;
            }
        }

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int Offset { get; }

        public LoadHalfWordAndZeroInstruction(int address, int registerDestination, int registerA, int offset)
        {
            Address = address;
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            Offset = offset;
        }
    }
}
