namespace GekkoAssembler
{
    public class MoveFromLinkRegisterInstruction : IGekkoInstruction
    {
        private MoveFromSpecialPurposeRegisterInstruction InternalInstruction { get; }

        public int Address { get { return InternalInstruction.Address; } }
        public int ByteCode { get { return InternalInstruction.ByteCode; } }

        public int RegisterDestination { get { return InternalInstruction.RegisterDestination; } }

        public MoveFromLinkRegisterInstruction(int address, int registerDestination)
        {
            InternalInstruction = new MoveFromSpecialPurposeRegisterInstruction(address, registerDestination, (int)SpecialPurposeRegister.LR);
        }
    }
}
