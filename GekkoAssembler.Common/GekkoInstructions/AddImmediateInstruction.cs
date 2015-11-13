namespace GekkoAssembler.GekkoInstructions
{
    public class AddImmediateInstruction : GekkoInstruction
    {
        public override int ByteCode
            => (((((14 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | (SIMM & 0xFFFF);

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int SIMM { get; }

        public AddImmediateInstruction(int address, int registerDestination, int registerA, int simm) : base(address)
        {
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            SIMM = simm;
        }
    }
}
