namespace GekkoAssembler
{
    public class LoadImmediateShiftedInstruction : IGekkoInstruction
    {
        private AddImmediateShiftedInstruction InternalInstruction { get; }

        public int Address { get { return InternalInstruction.Address; } }
        public int ByteCode { get { return InternalInstruction.ByteCode; } }

        public int RegisterDestination { get { return InternalInstruction.RegisterDestination; } }
        public int SIMM { get { return InternalInstruction.SIMM; } }

        public LoadImmediateShiftedInstruction(int address, int registerDestination, int simm)
        {
            InternalInstruction = new AddImmediateShiftedInstruction(address, registerDestination, 0, simm);
        }
    }
}
