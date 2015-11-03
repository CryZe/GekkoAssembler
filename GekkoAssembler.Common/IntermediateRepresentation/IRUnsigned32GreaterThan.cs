namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned32GreaterThan : IIRUnit
    {
        public int Address { get; }
        public uint Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRUnsigned32GreaterThan(int address, uint value, IRCodeBlock conditionalCode)
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
