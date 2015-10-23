using System;

namespace GekkoAssembler
{
    public class Unsigned32DataSection : IGekkoDataSection
    {
        public int Address { get; }
        public byte[] Data => BitConverter.GetBytes(Value).SwapEndian();
        public uint Value { get; }

        public Unsigned32DataSection(int address, uint value)
        {
            Address = address;
            Value = value;
        }
    }
}
