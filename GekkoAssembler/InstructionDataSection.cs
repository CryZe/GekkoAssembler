using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class InstructionDataSection : IGekkoDataSection
    {
        public int Address => Instruction.Address;
        public byte[] Data => BitConverter.GetBytes(Instruction.ByteCode).SwapEndian();
        public IGekkoInstruction Instruction { get; }

        public InstructionDataSection(IGekkoInstruction instruction)
        {
            Instruction = instruction;
        }
    }
}
