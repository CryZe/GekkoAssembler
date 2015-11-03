using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
{
    public interface ICodeWriter
    {
        Code WriteCode(IRCodeBlock block);
    }
}
