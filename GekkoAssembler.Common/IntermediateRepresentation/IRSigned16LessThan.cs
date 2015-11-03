namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRSigned16LessThan : IIRUnit
    {
        public int Address { get; }
        public short Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRSigned16LessThan(int address, short value, IRCodeBlock conditionalCode)
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
