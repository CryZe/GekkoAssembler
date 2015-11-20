namespace GekkoAssembler.GekkoInstructions
{
    public sealed class StoreHalfWordInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            STH  = 44,
            STHU = 45
        }

        public override int ByteCode { get; }

        public StoreHalfWordInstruction(int address, int rS, int offset, int rA, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | rS << 21 | rA << 16 | offset & 0xFFFF);
        }
    }
}
