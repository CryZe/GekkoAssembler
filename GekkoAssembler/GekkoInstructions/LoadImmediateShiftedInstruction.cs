namespace GekkoAssembler.GekkoInstructions
{
    public class LoadImmediateShiftedInstruction : AddImmediateShiftedInstruction
    {
        public LoadImmediateShiftedInstruction(int address, int registerDestination, int simm)
            : base(address, registerDestination, 0, simm)
        { }
    }
}
