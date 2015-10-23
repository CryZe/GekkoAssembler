namespace GekkoAssembler
{
    public class ConditionRegisterClearInstruction : IGekkoInstruction
    {
        private ConditionRegisterXORInstruction InternalInstruction { get; }

        public int Address { get { return InternalInstruction.Address; } }
        public int ByteCode { get { return InternalInstruction.ByteCode; } }

        public int ConditionRegisterDestination { get { return InternalInstruction.ConditionRegisterDestination; } }

        public ConditionRegisterClearInstruction(int address, int conditionRegisterDestination)
        {
            InternalInstruction = new ConditionRegisterXORInstruction(address, conditionRegisterDestination, conditionRegisterDestination, conditionRegisterDestination);
        }
    }
}
