namespace GekkoAssembler.DataSections
{
    public sealed class Unsigned8DataSection : GekkoDataSection<byte>
    {
        public override byte[] Data => new[] { Value };

        public Unsigned8DataSection(int address, byte value) : base(address, value)
        {
        }
    }
}
