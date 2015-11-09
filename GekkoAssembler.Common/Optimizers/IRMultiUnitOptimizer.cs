using System.Collections.Generic;
using System.Linq;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Optimizers
{
    public class IRMultiUnitOptimizer : IOptimizer
    {
        public IRCodeBlock Optimize(IRCodeBlock block)
        {
            var units = block.Units;

            var newUnits = units
                .SelectMany(x => x is IRMultiUnit ? (x as IRMultiUnit).Inner : (IEnumerable<IIRUnit>)new[] { x })
                .Select(x => x is IRCodeBlock ? Optimize(x as IRCodeBlock) : x);

            return newUnits.ToCodeBlock();
        }
    }
}
