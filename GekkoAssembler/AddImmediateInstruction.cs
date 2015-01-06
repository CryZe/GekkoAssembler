using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class AddImmediateInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((14 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | (SIMM & 0xFFFF);
            }
        }

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int SIMM { get; }

        public AddImmediateInstruction(int address, int registerDestination, int registerA, int simm)
        {
            Address = address;
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            SIMM = simm;
        }
    }
}
