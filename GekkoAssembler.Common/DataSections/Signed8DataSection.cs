namespace GekkoAssembler.DataSections
{
    public sealed class Signed8DataSection : GekkoDataSection<sbyte>
    {
        public override byte[] Data => new[] { (byte)Value };

        public Signed8DataSection(int address, sbyte value) : base(address, value)
        {
        }
    }
}
