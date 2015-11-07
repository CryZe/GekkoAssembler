using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Signed16DataSection : GekkoDataSection<short>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian16();

        public Signed16DataSection(int address, short value) : base(address, value)
        {
        }
    }
}
