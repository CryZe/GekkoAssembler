namespace GekkoAssembler.GekkoInstructions
{
    public sealed class StoreByteInstruction : GekkoInstruction
    {
        public enum Opcode
        {
            STB  = 38,
            STBU = 39
        }

        public override int ByteCode { get; }

        public StoreByteInstruction(int address, int rS, int offset, int rA, Opcode opcode) : base(address)
        {
            ByteCode = ((int)opcode << 26 | rS << 21 | rA << 16 | offset & 0xFFFF);
        }
    }
}
