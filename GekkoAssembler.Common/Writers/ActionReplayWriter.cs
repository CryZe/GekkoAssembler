using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
{
    public class ActionReplayWriter : ICodeWriter
    {
        public Code WriteCode(IRCodeBlock block)
        {
            var builder = new CodeBuilder();
            var visitor = new ActionReplayVisitor(builder);
            visitor.Visit(block);

            return builder.Build();
        }
    }
}
