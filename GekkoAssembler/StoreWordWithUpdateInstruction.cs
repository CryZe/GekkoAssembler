namespace GekkoAssembler
{
    public class StoreWordWithUpdateInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((37 << 5) | RegisterSource) << 5) | (RegisterAddress & 0x1F)) << 16) | (Offset & 0xFFFF);
            }
        }

        public int RegisterSource { get; }
        public int RegisterAddress { get; }
        public int Offset { get; }

        public StoreWordWithUpdateInstruction(int address, int registerSource, int offset, int registerAddress)
        {
            Address = address;
            RegisterSource = registerSource;
            RegisterAddress = registerAddress;
            Offset = offset;
        }
    }
}
