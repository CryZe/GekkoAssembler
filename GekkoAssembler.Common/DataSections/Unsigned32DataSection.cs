using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Unsigned32DataSection : GekkoDataSection<uint>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian32();

        public Unsigned32DataSection(int address, uint value) : base (address, value)
        {
        }
    }
}
