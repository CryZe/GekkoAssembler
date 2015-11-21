using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
{
    public class ByteCodeWriter : ICodeWriter
    {
        public Code WriteCode(IRCodeBlock block)
        {
            var builder = new CodeBuilder();
            var visitor = new ByteCodeVisitor(builder);
            visitor.Visit(block);

            return builder.Build();
        }
    }
}