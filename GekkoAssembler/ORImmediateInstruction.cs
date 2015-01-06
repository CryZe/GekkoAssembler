using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class OrImmediateInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((24 << 5) | RegisterSource) << 5) | RegisterA) << 16) | UIMM;
            }
        }

        public int RegisterA { get; }
        public int RegisterSource { get; }
        public int UIMM { get; }

        public OrImmediateInstruction(int address, int registerA, int registerSource, int uimm)
        {
            Address = address;
            RegisterA = registerA;
            RegisterSource = registerSource;
            UIMM = uimm;
        }
    }
}
