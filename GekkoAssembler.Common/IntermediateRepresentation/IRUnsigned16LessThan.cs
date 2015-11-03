namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned16LessThan : IIRUnit
    {
        public int Address { get; }
        public ushort Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRUnsigned16LessThan(int address, ushort value, IRCodeBlock conditionalCode)
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
