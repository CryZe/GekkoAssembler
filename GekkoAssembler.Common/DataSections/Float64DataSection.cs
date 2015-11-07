using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Float64DataSection : GekkoDataSection<double>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian64();

        public Float64DataSection(int address, double value) : base(address, value)
        {
        }
    }
}
