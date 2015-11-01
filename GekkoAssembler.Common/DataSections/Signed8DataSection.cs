namespace GekkoAssembler.DataSections
{
    public class Signed8DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => new byte[] { (byte)Value };
        public sbyte Value { get; }

        public Signed8DataSection(int address, sbyte value)
        {
            Address = address;
            Value = value;
        }
    }
}
