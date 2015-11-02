namespace GekkoAssembler.IntermediateRepresentation
{
    public interface IIRUnitVisitor
    {
        void Visit(IRWriteData instruction);
        void Visit(IRCodeBlock block);

        void Visit(IRUnsigned8Equal instruction);
        void Visit(IRUnsigned16Equal instruction);
        void Visit(IRUnsigned32Equal instruction);
        void Visit(IRSigned8Equal instruction);
        void Visit(IRSigned16Equal instruction);
        void Visit(IRSigned32Equal instruction);
        void Visit(IRFloat32Equal instruction);

        void Visit(IRUnsigned8Add instruction);
        void Visit(IRUnsigned16Add instruction);
        void Visit(IRUnsigned32Add instruction);
        void Visit(IRSigned8Add instruction);
        void Visit(IRSigned16Add instruction);
        void Visit(IRSigned32Add instruction);
        void Visit(IRFloat32Add instruction);
    }
}
