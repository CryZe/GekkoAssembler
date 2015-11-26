namespace GekkoAssembler.GekkoInstructions
{
    public sealed class StoreFloatingPointIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            STFDUX = 759,
            STFDX  = 727,
            STFIWX = 983,
            STFSUX = 695,
            STFSX  = 663
        }

        public override int ByteCode { get; }

        public StoreFloatingPointIndexedInstruction(int address, int frS, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | frS << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
