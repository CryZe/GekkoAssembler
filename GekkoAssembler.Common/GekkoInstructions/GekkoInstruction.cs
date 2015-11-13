using System;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.GekkoInstructions
{
    public abstract class GekkoInstruction : IRWriteData
    {
        public abstract int ByteCode { get; }

        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(ByteCode).SwapEndian32();

        protected GekkoInstruction(int address)
        {
            Address = address;
        }
    }
}
