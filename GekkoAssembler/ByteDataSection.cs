using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class ByteDataSection : IGekkoDataSection
    {
        public int Address { get; }
        public byte[] Data => new [] { Value };
        public byte Value { get; }

        public ByteDataSection(int address, byte value)
        {
            Address = address;
            Value = value;
        }
    }
}
