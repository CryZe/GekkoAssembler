using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
{
    public class GeckoWriter : ICodeWriter
    {
        public Code WriteCode(IRCodeBlock block)
        {
            var builder = new CodeBuilder();
            var visitor = new GeckoVisitor(builder);
            visitor.Visit(block);

            return builder.Build();
        }
    }
}
