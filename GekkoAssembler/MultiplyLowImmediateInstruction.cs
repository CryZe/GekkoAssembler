namespace GekkoAssembler
{
    public class MultiplyLowImmediateInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((7 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | (SIMM & 0xFFFF);
            }
        }

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int SIMM { get; }

        public MultiplyLowImmediateInstruction(int address, int registerDestination, int registerA, int simm)
        {
            Address = address;
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            SIMM = simm;
        }
    }
}
