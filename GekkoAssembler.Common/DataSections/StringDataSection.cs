using System.Text;

namespace GekkoAssembler.DataSections
{
    public sealed class StringDataSection : GekkoDataSection<string>
    {
        public override byte[] Data => Encoding.UTF8.GetBytes(Value);

        public StringDataSection(int address, string text) : base(address, text)
        {
        }
    }
}
