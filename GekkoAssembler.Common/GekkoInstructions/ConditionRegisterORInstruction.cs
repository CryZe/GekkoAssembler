namespace GekkoAssembler.GekkoInstructions
{
    public sealed class ConditionRegisterORInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public ConditionRegisterORInstruction(int address, int crD, int crA, int crB) : base(address)
        {
            this.ByteCode = (19 << 26 | crD << 21 | crA << 16 | crB << 11 | 449 << 1);
        }
    }
}
