namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned8Equal : IIRUnit
    {
        public int Address { get; }
        public byte Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRUnsigned8Equal(int address, byte value, IRCodeBlock conditionalCode)
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
