using System;

namespace GekkoAssembler.DataSections
{
    public class Float32DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian32();
        public float Value { get; }

        public Float32DataSection(int address, float value)
        {
            Address = address;
            Value = value;
        }
    }
}
