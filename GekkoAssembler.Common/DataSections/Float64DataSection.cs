using System;

namespace GekkoAssembler.DataSections
{
    public class Float64DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian64();
        public double Value { get; }

        public Float64DataSection(int address, double value)
        {
            Address = address;
            Value = value;
        }
    }
}
