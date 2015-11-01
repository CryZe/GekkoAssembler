namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned8Add : IIRUnit
    {
        public int Address { get; }
        public byte Value { get; }

        public IRUnsigned8Add(int address, byte value)
        {
            Address = address;
            Value = value;
        }

        public void Accept(IIRUnitVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
