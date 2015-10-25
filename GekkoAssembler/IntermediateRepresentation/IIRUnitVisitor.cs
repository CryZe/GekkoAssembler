namespace GekkoAssembler.IntermediateRepresentation
{
    public interface IIRUnitVisitor
    {
        void Visit(IRWriteData instruction);
    }
}
