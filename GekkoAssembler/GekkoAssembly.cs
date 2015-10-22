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
		
		public void Add(IGekkoInstruction instruction)
		{
			DataSections.Add(new InstructionDataSection(instruction));
		}
		
		public string ToCheat()
		{
			return DataSections.ToCheat();
		}
	}
}