using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Unsigned64DataSection : GekkoDataSection<ulong>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian64();

        public Unsigned64DataSection(int address, ulong value) : base(address, value)
        {
        }
    }
}
