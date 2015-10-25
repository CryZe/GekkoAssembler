using System;

namespace GekkoAssembler.DataSections
{
    public class Unsigned64DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian64();
        public ulong Value { get; }

        public Unsigned64DataSection(int address, ulong value)
        {
            Address = address;
            Value = value;
        }
    }
}
