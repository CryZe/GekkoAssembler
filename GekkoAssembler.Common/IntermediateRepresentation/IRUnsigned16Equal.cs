namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned16Equal : IIRUnit
    {
        public int Address { get; }
        public ushort Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRUnsigned16Equal(int address, ushort value, IRCodeBlock conditionalCode)
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
