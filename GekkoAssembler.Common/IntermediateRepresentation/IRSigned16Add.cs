namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRSigned16Add : IIRUnit
    {
        public int Address { get; }
        public short Value { get; }

        public IRSigned16Add(int address, short value)
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
