using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler
{
    public static class Extensions
    {
        public static byte[] SwapEndian16(this byte[] data)
        {
            var temp = data[0];
            data[0] = data[1];
            data[1] = temp;
            return data;
        }

        public static byte[] SwapEndian32(this byte[] data)
        {
            var temp = data[0];
            data[0] = data[3];
            data[3] = temp;
            temp = data[1];
            data[1] = data[2];
            data[2] = temp;
            return data;
        }

        public static byte[] SwapEndian64(this byte[] data)
        {
            var temp = data[0];
            data[0] = data[7];
            data[7] = temp;
            temp = data[1];
            data[1] = data[6];
            data[6] = temp;
            temp = data[2];
            data[2] = data[5];
            data[5] = temp;
            temp = data[3];
            data[3] = data[4];
            data[4] = temp;
            return data;
        }

        public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        public static IEnumerable<IIRUnit> Replace<T>(this IEnumerable<IIRUnit> enumerable, Func<T, IIRUnit> selector)
            where T : IIRUnit
        {
            return enumerable.Replace(selector, x => true);
        }

        public static IEnumerable<IIRUnit> Replace<T>(this IEnumerable<IIRUnit> enumerable, Func<T, IIRUnit> selector, Func<T, bool> predicate)
            where T : IIRUnit
        {
            return enumerable
                .Select(x => x is IRCodeBlock ? (x as IRCodeBlock).Units.Replace(selector, predicate).ToCodeBlock() : x)
                .Select(x => x is T && predicate((T)x) ? selector((T)x) : x);
        }

        public static IRCodeBlock ToCodeBlock(this IEnumerable<IIRUnit> units)
        {
            return new IRCodeBlock(units);
        }
    }
}