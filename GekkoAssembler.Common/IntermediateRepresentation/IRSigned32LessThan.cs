namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRSigned32LessThan : IIRUnit
    {
        public int Address { get; }
        public int Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRSigned32LessThan(int address, int value, IRCodeBlock conditionalCode)
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
