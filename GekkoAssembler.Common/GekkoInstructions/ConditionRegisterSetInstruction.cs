namespace GekkoAssembler.GekkoInstructions
{
    public class ConditionRegisterSetInstruction : ConditionRegisterEquivalentInstruction
    {
        public ConditionRegisterSetInstruction(int address, int conditionRegisterDestination)
            : base(address, conditionRegisterDestination, conditionRegisterDestination, conditionRegisterDestination)
        { }
    }
}
