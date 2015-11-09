using System;
using System.Collections.Generic;
using System.Linq;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Optimizers
{
    public class WriteDataOptimizer : IOptimizer
    {
        private IRCodeBlock Merge(IRCodeBlock block)
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

                        var consecutive = currentWriteData.Address == lastWriteData.Address + lastWriteData.Length;

                        if (consecutive)
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

            return units.Select(x => x is IRCodeBlock ? Merge(x as IRCodeBlock) : x).ToCodeBlock();
        }

        private IRCodeBlock FixAlignmentIssues(IRCodeBlock block)
        {
            var units = block.Units;

            units = units.SelectMany(x => x is IRWriteData ? FixAlignmentIssues(x as IRWriteData) : new[] { x }).AsReadOnly();
            
            return units.Select(x => x is IRCodeBlock ? FixAlignmentIssues(x as IRCodeBlock) : x).ToCodeBlock();
        }

        private IEnumerable<IIRUnit> FixAlignmentIssues(IRWriteData dataSection)
        {
            if (dataSection.Address % 2 != 0 && dataSection.Length >= 2)
            {
                var splitted = Split(dataSection, 1);
                yield return splitted.Item1;
                dataSection = splitted.Item2;
            }
            if (dataSection.Address % 4 != 0 && dataSection.Length >= 4)
            {
                var splitted = Split(dataSection, 2);
                yield return splitted.Item1;
                dataSection = splitted.Item2;
            }

            yield return dataSection;
        }

        private Tuple<IRWriteData, IRWriteData> Split(IRWriteData dataSection, int length)
        {
            var firstArr = new byte[length];
            var secondArr = new byte[dataSection.Length - length];

            Array.Copy(dataSection.Data, firstArr, length);
            Array.Copy(dataSection.Data, length, secondArr, 0, secondArr.Length);

            var first = new CustomIRWriteData(dataSection.Address, firstArr);
            var second = new CustomIRWriteData(dataSection.Address + length, secondArr);

            return new Tuple<IRWriteData, IRWriteData>(first, second);
        }

        public IRCodeBlock Optimize(IRCodeBlock block)
        {
            var merged = Merge(block);

            return FixAlignmentIssues(merged);
        }
    }
}
