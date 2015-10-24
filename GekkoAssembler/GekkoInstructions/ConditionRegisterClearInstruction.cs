namespace GekkoAssembler
{
    public class ConditionRegisterClearInstruction : ConditionRegisterXORInstruction
    {
        public ConditionRegisterClearInstruction(int address, int conditionRegisterDestination)
            : base(address, conditionRegisterDestination, conditionRegisterDestination, conditionRegisterDestination)
        { }
    }
}
