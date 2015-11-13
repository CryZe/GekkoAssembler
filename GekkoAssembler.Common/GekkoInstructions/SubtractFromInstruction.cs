﻿namespace GekkoAssembler.GekkoInstructions
{
    public class SubtractFromInstruction : GekkoInstruction
    {
        public override int ByteCode
            => 0x7C000050 | (RegisterDestination << 21) | (RegisterA << 16) | (RegisterB << 11) | ((OE ? 1 : 0) << 10) | (RC ? 1 : 0);

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int RegisterB { get; }
        public bool OE { get; }
        public bool RC { get; }

        public SubtractFromInstruction(int address, int registerDestination, int registerA, int registerB, bool oe = false, bool rc = false) : base(address)
        {
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            RegisterB = registerB;
            OE = oe;
            RC = rc;
        }
    }
}
