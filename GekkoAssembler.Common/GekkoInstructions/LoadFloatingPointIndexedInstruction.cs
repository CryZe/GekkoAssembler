namespace GekkoAssembler.GekkoInstructions
{
    public sealed class LoadFloatingPointIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            LFDUX = 631,
            LFDX  = 599,
            LFSUX = 567,
            LFSX  = 535
        }

        public override int ByteCode { get; }

        public LoadFloatingPointIndexedInstruction(int address, int frD, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | frD << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
