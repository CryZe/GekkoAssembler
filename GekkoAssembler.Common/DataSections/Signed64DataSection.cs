using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Signed64DataSection : GekkoDataSection<long>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian64();

        public Signed64DataSection(int address, long value) : base(address, value)
        {
        }
    }
}
