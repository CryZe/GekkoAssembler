using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class MoveFromSpecialPurposeRegisterInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return 0x7C0002A6 | (RegisterDestination << 21) | ((SpecialPurposeRegister & 0x1F) << 16) | ((SpecialPurposeRegister >> 5) << 11);
            }
        }

        public int RegisterDestination { get; }
        public int SpecialPurposeRegister { get; }

        public MoveFromSpecialPurposeRegisterInstruction(int address, int registerDestination, int specialPurposeRegister)
        {
            Address = address;
            RegisterDestination = registerDestination;
            SpecialPurposeRegister = specialPurposeRegister;
        }
    }
}
