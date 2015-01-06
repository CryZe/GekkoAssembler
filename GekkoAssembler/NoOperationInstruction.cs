using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class NoOperationInstruction : IGekkoInstruction
    {
        public int Address { get { return InternalInstruction.Address; } }

        public int ByteCode { get { return InternalInstruction.ByteCode; } }

        private OrImmediateInstruction InternalInstruction { get; }

        public NoOperationInstruction(int address)
        {
            InternalInstruction = new OrImmediateInstruction(address, 0, 0, 0);
        }
    }
}
