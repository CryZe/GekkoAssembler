using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Optimizers
{
    public interface IOptimizer
    {
        IRCodeBlock Optimize(IRCodeBlock block);
    }
}
