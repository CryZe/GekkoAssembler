using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public interface IGekkoDataSection
    {
		uint Address { get; }
		byte[] Data { get; }
	}
	
	public static class IGekkoDataSectionExtensions
	{
		public static string ToCheat(this IGekkoDataSection dataSection)
        {
			var lines = new List<string>();
			uint i = 0;
			
			while ((i + 4) <= dataSection.Data.Length)
			{
				lines.Add(string.Format("04{0:X6} {1:X2}{2:X2}{3:X2}{4:X2}", (dataSection.Address & 0xFFFFFF), dataSection.Data[i], dataSection.Data[i + 1], dataSection.Data[i + 2], dataSection.Data[i + 3]));
				i += 4;
			}
			
			if ((i + 2) <= dataSection.Data.Length)
			{
				lines.Add(string.Format("02{0:X6} 0000{1:X2}{2:X2}", (dataSection.Address & 0xFFFFFF), dataSection.Data[i], dataSection.Data[i + 1]));
				i += 4;
			}
			
			if (i < dataSection.Data.Length)
			{
				lines.Add(string.Format("00{0:X6} 000000{1:X2}", (dataSection.Address & 0xFFFFFF), dataSection.Data[i]));
			}
			
            return string.Join(Environment.NewLine, lines);
        }
	}
}