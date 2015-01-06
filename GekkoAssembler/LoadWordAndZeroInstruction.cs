using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class LoadWordAndZeroInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (((((32 << 5) | RegisterDestination) << 5) | RegisterA) << 16) | Offset;
            }
        }

        public int RegisterDestination { get; }
        public int RegisterA { get; }
        public int Offset { get; }

        public LoadWordAndZeroInstruction(int address, int registerDestination, int registerA, int offset)
        {
            Address = address;
            RegisterDestination = registerDestination;
            RegisterA = registerA;
            Offset = offset;
        }
    }
}
