using System;

namespace GekkoAssembler
{
    public class InstructionDataSection : GekkoDataSection
    {
        public override int Address => Instruction.Address;
        public override byte[] Data => BitConverter.GetBytes(Instruction.ByteCode).SwapEndian32();
        public GekkoInstruction Instruction { get; }

        public InstructionDataSection(GekkoInstruction instruction)
        {
            Instruction = instruction;
        }
    }
}
