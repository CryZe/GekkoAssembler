namespace GekkoAssembler.DataSections
{
    public class Unsigned8DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => new byte[] { Value };
        public byte Value { get; }

        public Unsigned8DataSection(int address, byte value)
        {
            Address = address;
            Value = value;
        }
    }
}
