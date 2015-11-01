namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRSigned32Add : IIRUnit
    {
        public int Address { get; }
        public int Value { get; }

        public IRSigned32Add(int address, int value)
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
