namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned8BitSet : IIRUnit
    {
        public int Address { get; }
        public byte Value { get; }

        public IRUnsigned8BitSet(int address, byte value)
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
