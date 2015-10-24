using System;

namespace GekkoAssembler
{
    public abstract class GekkoInstruction : IRWriteData
    {
        public abstract int ByteCode { get; }

        public override byte[] Data => BitConverter.GetBytes(ByteCode).SwapEndian32();
    }
}
