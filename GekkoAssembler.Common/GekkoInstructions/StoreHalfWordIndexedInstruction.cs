namespace GekkoAssembler.GekkoInstructions
{
    public sealed class StoreHalfWordIndexedInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            STHBRX = 918,
            STHUX  = 439,
            STHX   = 407
        }

        public override int ByteCode { get; }

        public StoreHalfWordIndexedInstruction(int address, int rS, int rA, int rB, Opcode opcode) : base(address)
        {
            ByteCode = (31 << 26 | rS << 21 | rA << 16 | rB << 11 | (int)opcode << 1);
        }
    }
}
