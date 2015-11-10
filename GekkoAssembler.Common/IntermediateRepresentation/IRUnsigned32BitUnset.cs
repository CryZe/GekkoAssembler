namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRUnsigned32BitUnset : IIRUnit
    {
        public int Address { get; }
        public uint Value { get; }

        public IRUnsigned32BitUnset(int address, uint value)
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
