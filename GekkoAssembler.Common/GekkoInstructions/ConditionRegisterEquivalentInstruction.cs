namespace GekkoAssembler.GekkoInstructions
{
    public class ConditionRegisterEquivalentInstruction : GekkoInstruction
    {
        public override int ByteCode
            => (((((((((19 << 5) | ConditionRegisterDestination) << 5) | ConditionRegisterA) << 5) | ConditionRegisterB) << 10) | 289) << 1);

        public int ConditionRegisterDestination { get; }
        public int ConditionRegisterA { get; }
        public int ConditionRegisterB { get; }

        public ConditionRegisterEquivalentInstruction(int address, int conditionRegisterDestination, int conditionRegisterA, int conditionRegisterB) : base(address)
        {
            ConditionRegisterDestination = conditionRegisterDestination;
            ConditionRegisterA = conditionRegisterA;
            ConditionRegisterB = conditionRegisterB;
        }
    }
}
