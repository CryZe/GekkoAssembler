namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned16Add : IIRUnit
    {
        public int Address { get; }
        public ushort Value { get; }

        public IRUnsigned16Add(int address, ushort value)
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
