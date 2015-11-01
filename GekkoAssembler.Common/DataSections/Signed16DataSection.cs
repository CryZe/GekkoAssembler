using System;

namespace GekkoAssembler.DataSections
{
    public class Signed16DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian16();
        public short Value { get; }

        public Signed16DataSection(int address, short value)
        {
            Address = address;
            Value = value;
        }
    }
}
