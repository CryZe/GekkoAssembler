﻿namespace GekkoAssembler.GekkoInstructions
{
    public class ConditionRegisterXORInstruction : GekkoInstruction
    {
        public override int ByteCode
            => (((((((((19 << 5) | ConditionRegisterDestination) << 5) | ConditionRegisterA) << 5) | ConditionRegisterB) << 10) | 193) << 1);

        public int ConditionRegisterDestination { get; }
        public int ConditionRegisterA { get; }
        public int ConditionRegisterB { get; }

        public ConditionRegisterXORInstruction(int address, int conditionRegisterDestination, int conditionRegisterA, int conditionRegisterB) : base(address)
        {
            ConditionRegisterDestination = conditionRegisterDestination;
            ConditionRegisterA = conditionRegisterA;
            ConditionRegisterB = conditionRegisterB;
        }
    }
}
