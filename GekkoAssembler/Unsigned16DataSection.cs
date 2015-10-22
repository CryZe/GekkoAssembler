using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class Unsigned16DataSection : IGekkoDataSection
    {
        public int Address { get; }
        public byte[] Data => BitConverter.GetBytes(Value).SwapEndian();
		public ushort Value { get; }
		
		public Unsigned16DataSection(int address, ushort value)
		{
			Address = address;
			Value = value;
		}
	}
}