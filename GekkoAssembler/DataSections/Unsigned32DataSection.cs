using System;

namespace GekkoAssembler
{
    public class Unsigned32DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian32();
        public uint Value { get; }

        public Unsigned32DataSection(int address, uint value)
        {
            Address = address;
            Value = value;
        }
    }
}
