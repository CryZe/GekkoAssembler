﻿namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned8Add : IIRUnit
    {
        public int Address { get; }
        public byte Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRUnsigned8Add(int address, byte value, IRCodeBlock conditionalCode)
        {
            Address = address;
            Value = value;
            ConditionalCode = conditionalCode;
        }

        public void Accept(IIRUnitVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
