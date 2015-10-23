namespace GekkoAssembler
{
    public class MoveToSpecialPurposeRegisterInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return 0x7C0003A6 | (RegisterSource << 21) | ((SpecialPurposeRegisterID & 0x1F) << 16) | ((SpecialPurposeRegisterID >> 5) << 11);
            }
        }

        public int SpecialPurposeRegisterID { get; }
        public int RegisterSource { get; }

        public MoveToSpecialPurposeRegisterInstruction(int address, int specialPurposeRegister, int registerSource)
        {
            Address = address;
            RegisterSource = registerSource;
            SpecialPurposeRegisterID = specialPurposeRegister;
        }
    }
}
