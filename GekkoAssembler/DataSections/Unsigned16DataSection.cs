using System;

namespace GekkoAssembler
{
    public class Unsigned16DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian16();
        public ushort Value { get; }

        public Unsigned16DataSection(int address, ushort value)
        {
            Address = address;
            Value = value;
        }
    }
}
