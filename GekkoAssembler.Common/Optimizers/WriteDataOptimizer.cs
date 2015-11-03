using System.Linq;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Optimizers
{
    public class WriteDataOptimizer : IOptimizer
    {
        public IRCodeBlock Optimize(IRCodeBlock block)
        {
            var units = block.Units;

            var replaced = true;
            while (replaced)
            {
                replaced = false;

                var last = units.FirstOrDefault();
                foreach (var current in units.Skip(1))
                {
                    if (current is IRWriteData && last is IRWriteData)
                    {
                        var currentWriteData = current as IRWriteData;
                        var lastWriteData = last as IRWriteData;

                        if (currentWriteData.Address == lastWriteData.Address + lastWriteData.Data.Length)
                        {
                            var combined = new IRCombinedWriteData(lastWriteData, currentWriteData);
                            units = units.TakeWhile(x => x != last)
                                        .Concat(new[] { combined })
                                        .Concat(units.SkipWhile(x => x != current).Skip(1))
                                        .AsReadOnly();
                            replaced = true;
                            break;
                        }
                    }
                    last = current;
                }
            }

            return new IRCodeBlock(units.Select(x => x is IRCodeBlock ? Optimize(x as IRCodeBlock) : x));
        }
    }
}
