namespace GekkoAssembler.GekkoInstructions
{
    public sealed class ConditionRegisterANDComplementInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        public ConditionRegisterANDComplementInstruction(int address, int crD, int crA, int crB) : base(address)
        {
            ByteCode = (19 << 26 | crD << 21 | crA << 16 | crB << 11 | 129 << 1);
        }
    }
}
