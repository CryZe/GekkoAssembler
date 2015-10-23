using System;

namespace GekkoAssembler
{
    public static class EndianExtensions
    {
        public static byte[] SwapEndian(this byte[] data)
        {
            if (data.Length == 2)
            {
                var temp = data[0];
                data[0] = data[1];
                data[1] = temp;
                return data; 
            }
            else
            {
                var temp = data[0];
                data[0] = data[3];
                data[3] = temp;
                temp = data[1];
                data[1] = data[2];
                data[2] = temp;
                return data; 
            }
        }
    }
}