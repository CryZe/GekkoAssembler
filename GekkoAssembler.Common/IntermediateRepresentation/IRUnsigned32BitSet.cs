namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned32BitSet : IIRUnit
    {
        public int Address { get; }
        public uint Value { get; }

        public IRUnsigned32BitSet(int address, uint value)
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
