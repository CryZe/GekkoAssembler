using System;

namespace GekkoAssembler.DataSections
{
    public class Signed64DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian64();
        public long Value { get; }

        public Signed64DataSection(int address, long value)
        {
            Address = address;
            Value = value;
        }
    }
}
