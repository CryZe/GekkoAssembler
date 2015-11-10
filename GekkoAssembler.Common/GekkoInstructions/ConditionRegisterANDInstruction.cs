namespace GekkoAssembler.GekkoInstructions
{
    public sealed class ConditionRegisterANDInstruction : GekkoInstruction
    {
        public override int Address  { get; }
        public override int ByteCode { get; }

        public ConditionRegisterANDInstruction(int address, int crD, int crA, int crB)
        {
            this.Address = address;
            this.ByteCode = (19 << 26 | crD << 21 | crA << 16 | crB << 11 | 257 << 1);
        }
    }
}
