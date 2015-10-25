namespace GekkoAssembler.GekkoInstructions
{
    public class MoveToLinkRegisterInstruction : MoveToSpecialPurposeRegisterInstruction
    {
        public MoveToLinkRegisterInstruction(int address, int registerSource)
            : base(address, (int)SpecialPurposeRegister.LR, registerSource)
        { }
    }
}
