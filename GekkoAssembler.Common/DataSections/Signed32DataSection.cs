using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Signed32DataSection : GekkoDataSection<int>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian32();

        public Signed32DataSection(int address, int value) : base(address, value)
        {
        }
    }
}
