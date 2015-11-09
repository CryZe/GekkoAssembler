using GekkoAssembler.IntermediateRepresentation;
using GekkoAssembler.Optimizers;

namespace GekkoAssembler.Writers
{
    public class ActionReplayWriter : ICodeWriter
    {
        public Code WriteCode(IRCodeBlock block)
        {
            var builder = new CodeBuilder();
            var visitor = new ActionReplayVisitor(builder);
            var optimizer = new ActionReplayOptimizer(builder);

            block = optimizer.Optimize(block);

            visitor.Visit(block);

            return builder.Build();
        }
    }
}
