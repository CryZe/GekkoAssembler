namespace GekkoAssembler
{
    public class MoveToLinkRegisterInstruction : IGekkoInstruction
    {
        private MoveToSpecialPurposeRegisterInstruction InternalInstruction { get; }

        public int Address { get { return InternalInstruction.Address; } }
        public int ByteCode { get { return InternalInstruction.ByteCode; } }

        public int RegisterSource { get { return InternalInstruction.RegisterSource; } }

        public MoveToLinkRegisterInstruction(int address, int registerSource)
        {
            InternalInstruction = new MoveToSpecialPurposeRegisterInstruction(address, (int)SpecialPurposeRegister.LR, registerSource);
        }
    }
}
