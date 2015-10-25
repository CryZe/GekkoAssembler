namespace GekkoAssembler.GekkoInstructions
{
    public class StoreWordWithUpdateInstruction : GekkoInstruction
    {
        public override int Address { get; }
        public override int ByteCode
            => (((((37 << 5) | RegisterSource) << 5) | (RegisterAddress & 0x1F)) << 16) | (Offset & 0xFFFF);

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
