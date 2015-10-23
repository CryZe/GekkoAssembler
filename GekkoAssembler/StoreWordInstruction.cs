namespace GekkoAssembler
{
    public class StoreWordInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((36 << 5) | RegisterSource) << 5) | (RegisterAddress & 0x1F)) << 16) | (Offset & 0xFFFF);
            }
        }

        public int RegisterSource { get; }
        public int RegisterAddress { get; }
        public int Offset { get; }

        public StoreWordInstruction(int address, int registerSource, int offset, int registerAddress)
        {
            Address = address;
            RegisterSource = registerSource;
            RegisterAddress = registerAddress;
            Offset = offset;
        }
    }
}
