namespace GekkoAssembler.GekkoInstructions
{
    public sealed class MoveToConditionRegisterInstruction : GekkoInstruction
    {
        public override int ByteCode { get; }

        // Represents MCRXR
        public MoveToConditionRegisterInstruction(int address, int crfD) : base (address)
        {
            ByteCode = (31 << 26 | crfD << 23 | 512 << 1);
        }

        public MoveToConditionRegisterInstruction(int address, int crfD, int crfS, bool fromFPSCR) : base(address)
        {
            if (fromFPSCR)
                ByteCode = (63 << 26 | crfD << 23 | crfS << 18 | 64 << 1);
            else
                ByteCode = (19 << 26 | crfD << 23 | crfS << 18);
        }
    }
}
