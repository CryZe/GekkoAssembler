namespace GekkoAssembler
{
    public class MoveFromLinkRegisterInstruction : MoveFromSpecialPurposeRegisterInstruction
    {
        public MoveFromLinkRegisterInstruction(int address, int registerDestination)
            : base(address, registerDestination, (int)SpecialPurposeRegister.LR)
        { }
    }
}
