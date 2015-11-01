namespace GekkoAssembler.IntermediateRepresentation
{
    public interface IIRUnit
    {
        void Accept(IIRUnitVisitor visitor);
    }
}