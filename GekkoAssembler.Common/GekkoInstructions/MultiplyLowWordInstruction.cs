namespace GekkoAssembler.GekkoInstructions
{
    public class MultiplyLowWordInstruction : GekkoInstruction
    {
        public override int Address { get; }
        public override int ByteCode
            => 0x7C0001D6 | (RegisterDestination << 21) | (RegisterA << 16) | (RegisterB << 11) | ((OE ? 1 : 0) << 10) | (RC ? 1 : 0);

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int RegisterB { get; }
        public bool OE { get; }
        public bool RC { get; }

        public MultiplyLowWordInstruction(int address, int registerDestination, int registerA, int registerB, bool oe = false, bool rc = false)
        {
            Address = address;
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            RegisterB = registerB;
            OE = oe;
            RC = rc;
        }
    }
}
