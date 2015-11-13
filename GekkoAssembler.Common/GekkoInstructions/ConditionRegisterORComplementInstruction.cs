namespace GekkoAssembler.GekkoInstructions
{
    public sealed class ConditionRegisterORComplementInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public ConditionRegisterORComplementInstruction(int address, int crD, int crA, int crB) : base(address)
        {
            this.ByteCode = (19 << 26 | crD << 21 | crA << 16 | crB << 11 | 417 << 1);
        }
    }
}
