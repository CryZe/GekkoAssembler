using System;
using System.Collections.Generic;
using System.Linq;

namespace GekkoAssembler
{
    public interface IGekkoDataSection
    {
        int Address { get; }
        byte[] Data { get; }
    }
    
    public static class IGekkoDataSectionExtensions
    {
        public static string ToCheat(this IGekkoDataSection dataSection)
        {
            var lines = new List<string>();
            var i = 0;
            
            while ((i + 4) <= dataSection.Data.Length)
            {
                lines.Add(string.Format("04{0:X6} {1:X2}{2:X2}{3:X2}{4:X2}", ((dataSection.Address + i) & 0xFFFFFF), dataSection.Data[i], dataSection.Data[i + 1], dataSection.Data[i + 2], dataSection.Data[i + 3]));
                i += 4;
            }
            
            if ((i + 2) <= dataSection.Data.Length)
            {
                lines.Add(string.Format("02{0:X6} 0000{1:X2}{2:X2}", ((dataSection.Address + i) & 0xFFFFFF), dataSection.Data[i], dataSection.Data[i + 1]));
                i += 2;
            }
            
            if (i < dataSection.Data.Length)
            {
                lines.Add(string.Format("00{0:X6} 000000{1:X2}", ((dataSection.Address + i) & 0xFFFFFF), dataSection.Data[i]));
            }
            
            return string.Join(Environment.NewLine, lines);
        }
        
        public static string ToCheat(this IEnumerable<IGekkoDataSection> dataSections)
        {
            dataSections = mergeDataSections(dataSections);
            
            return string.Join(Environment.NewLine, dataSections.Select(x => x.ToCheat()));
        }

        private static IEnumerable<IGekkoDataSection> mergeDataSections(IEnumerable<IGekkoDataSection> dataSections)
        {
            var replaced = true;
            while (replaced)
            {
                replaced = false;
                
                var last = dataSections.FirstOrDefault();
                foreach (var current in dataSections.Skip(1))
                {
                    if (current.Address == last.Address + last.Data.Length)
                    {
                        var combined = new CombinedDataSection(last, current);
                        dataSections = dataSections.TakeWhile(x => x != last)
                                    .Concat(new [] { combined })
                                    .Concat(dataSections.SkipWhile(x => x != current).Skip(1))
                                    .ToList();
                        replaced = true;
                        break;
                    }
                    last = current;
                }
            }

            return dataSections;
        }
    }
}