namespace GekkoAssembler.IntermediateRepresentation
{
    public interface IIRUnitVisitor
    {
        void Visit(IRWriteData instruction);
        void Visit(IRCodeBlock block);
        void Visit(IRUnsigned8Equal instruction);
        void Visit(IRUnsigned16Equal instruction);
    }
}
