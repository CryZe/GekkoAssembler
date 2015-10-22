using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class GekkoAssembly
    {
		public IList<IGekkoDataSection> DataSections { get; private set; }
		public IList<IGekkoInstruction> Instructions { get; private set; }
		
		public GekkoAssembly()
		{
			DataSections = new List<IGekkoDataSection>();
			Instructions = new List<IGekkoInstruction>();
		}
		
		public void Add(IGekkoDataSection dataSection)
		{
			DataSections.Add(dataSection);
		}
		
		public void Add(IGekkoInstruction instructions)
		{
			Instructions.Add(instructions);
		}
		
		public string ToCheat()
		{
			return string.Join(Environment.NewLine, 
				new [] { DataSections.ToCheat(), Instructions.ToCheat() }
				.Where(x => !string.IsNullOrEmpty(x)));
		}
	}
}