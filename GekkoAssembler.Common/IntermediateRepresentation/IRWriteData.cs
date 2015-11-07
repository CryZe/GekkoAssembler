namespace GekkoAssembler.IntermediateRepresentation
{
    public abstract class IRWriteData : IIRUnit
    {
        public abstract int Address { get; }
        public abstract byte[] Data { get; }
        public int Length => Data.Length;

        public void Accept(IIRUnitVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
