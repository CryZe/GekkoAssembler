namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned32Add : IIRUnit
    {
        public int Address { get; }
        public uint Value { get; }

        public IRUnsigned32Add(int address, uint value)
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
