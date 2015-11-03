namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRFloat32GreaterThan : IIRUnit
    {
        public int Address { get; }
        public float Value { get; }

        public IRCodeBlock ConditionalCode { get; }

        public IRFloat32GreaterThan(int address, float value, IRCodeBlock conditionalCode)
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
