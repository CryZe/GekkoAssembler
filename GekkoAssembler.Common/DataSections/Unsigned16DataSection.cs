using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Unsigned16DataSection : GekkoDataSection<ushort>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian16();

        public Unsigned16DataSection(int address, ushort value) : base(address, value)
        {
        }
    }
}
