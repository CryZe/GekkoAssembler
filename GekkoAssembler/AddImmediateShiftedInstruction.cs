namespace GekkoAssembler
{
    public class AddImmediateShiftedInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((15 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | (SIMM & 0xFFFF);
            }
        }

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int SIMM { get; }

        public AddImmediateShiftedInstruction(int address, int registerDestination, int registerA, int simm)
        {
            Address = address;
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            SIMM = simm;
        }
    }
}
