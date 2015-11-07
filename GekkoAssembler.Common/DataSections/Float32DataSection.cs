using System;

namespace GekkoAssembler.DataSections
{
    public sealed class Float32DataSection : GekkoDataSection<float>
    {
        public override byte[] Data => BitConverter.GetBytes(Value).SwapEndian32();

        public Float32DataSection(int address, float value) : base(address, value)
        {
        }
    }
}
