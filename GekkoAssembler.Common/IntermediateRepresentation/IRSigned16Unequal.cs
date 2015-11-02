namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRSigned16Unequal : IIRUnit
    {
        public int Address { get; }
        public short Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRSigned16Unequal(int address, short value, IRCodeBlock conditionalCode)
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
