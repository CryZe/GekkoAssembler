namespace GekkoAssembler.DataSections
{
    public class ByteDataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => new [] { Value };
        public byte Value { get; }

        public ByteDataSection(int address, byte value)
        {
            Address = address;
            Value = value;
        }
    }
}
