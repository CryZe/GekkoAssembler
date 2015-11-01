namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRFloat32Add : IIRUnit
    {
        public int Address { get; }
        public float Value { get; }

        public IRFloat32Add(int address, float value)
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
