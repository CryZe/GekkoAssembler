using System.Collections.Generic;
using System.Collections.ObjectModel;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Optimizers
{
    public class CompositeOptimizer : IOptimizer
    {
        public ReadOnlyCollection<IOptimizer> Optimizers { get; }

        public CompositeOptimizer(params IOptimizer[] optimizers)
            : this((IEnumerable<IOptimizer>)optimizers)
        { }

        public CompositeOptimizer(IEnumerable<IOptimizer> optimizers)
        {
            Optimizers = optimizers.AsReadOnly();
        }

        public IRCodeBlock Optimize(IRCodeBlock block)
        {
            foreach (var optimizer in Optimizers)
                block = optimizer.Optimize(block);

            return block;
        }
    }
}
