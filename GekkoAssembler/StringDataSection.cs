using System.Text;

namespace GekkoAssembler
{
    public class StringDataSection : IGekkoDataSection
    {
        public int Address { get; }
        public byte[] Data => Encoding.UTF8.GetBytes(Text);
        public string Text { get; }

        public StringDataSection(int address, string text)
        {
            Address = address;
            Text = text;
        }
    }
}
