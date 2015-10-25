using System.Text;

namespace GekkoAssembler.DataSections
{
    public class StringDataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => Encoding.UTF8.GetBytes(Text);
        public string Text { get; }

        public StringDataSection(int address, string text)
        {
            Address = address;
            Text = text;
        }
    }
}
