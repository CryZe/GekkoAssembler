namespace GekkoAssembler.GekkoInstructions
{
    public class MoveFromSpecialPurposeRegisterInstruction : GekkoInstruction
    {
        public override int ByteCode
            => 0x7C0002A6 | (RegisterDestination << 21) | ((SpecialPurposeRegisterID & 0x1F) << 16) | ((SpecialPurposeRegisterID >> 5) << 11);

        public int RegisterDestination { get; }
        public int SpecialPurposeRegisterID { get; }

        public MoveFromSpecialPurposeRegisterInstruction(int address, int registerDestination, int specialPurposeRegister) : base(address)
        {
            RegisterDestination = registerDestination;
            SpecialPurposeRegisterID = specialPurposeRegister;
        }
    }
}
