using System;

namespace GekkoAssembler.DataSections
{
    public class Signed32DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian32();
        public int Value { get; }

        public Signed32DataSection(int address, int value)
        {
            Address = address;
            Value = value;
        }
    }
}
