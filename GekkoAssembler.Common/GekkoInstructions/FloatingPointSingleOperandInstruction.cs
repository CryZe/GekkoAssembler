namespace GekkoAssembler.GekkoInstructions
{
    public sealed class FloatingPointSingleOperandInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            FABS    = 264,
            FCTIW   = 14,
            FCTIWZ  = 15,
            FMR     = 72,
            FNABS   = 136,
            FNEG    = 40,
            FRES    = 24,
            FRSP    = 12,
            FRSQRTE = 26
        }

        public override int ByteCode { get; }

        public FloatingPointSingleOperandInstruction(int address, int frD, int frB, bool rc, Opcode opcode) : base(address)
        {
            ByteCode = (63 << 26 | frD << 21 | frB << 11 | (int)opcode << 1 | (rc ? 1 : 0));
        }
    }
}
