namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRSigned8Add : IIRUnit
    {
        public int Address { get; }
        public sbyte Value { get; }

        public IRSigned8Add(int address, sbyte value)
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
