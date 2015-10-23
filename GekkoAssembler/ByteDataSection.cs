namespace GekkoAssembler
{
    public class ByteDataSection : IGekkoDataSection
    {
        public int Address { get; }
        public byte[] Data => new [] { Value };
        public byte Value { get; }

        public ByteDataSection(int address, byte value)
        {
            Address = address;
            Value = value;
        }
    }
}
