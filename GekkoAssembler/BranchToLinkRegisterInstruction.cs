using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class BranchToLinkRegisterInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode { get { return 0x4E800020; } }

        public BranchToLinkRegisterInstruction(int address)
        {
            Address = address;
        }
    }
}
