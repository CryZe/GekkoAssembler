using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    }
}