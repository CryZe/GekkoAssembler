using System.Collections.Generic;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler
{
    public class GekkoAssembly
    {
        public IList<IIRUnit> Units { get; private set; }

        public GekkoAssembly()
        {
            Units = new List<IIRUnit>();
        }

        public GekkoAssembly(IEnumerable<IIRUnit> units)
        {
            Units = new List<IIRUnit>(units);
        }

        public void Add(IIRUnit unit)
        {
            Units.Add(unit);
        }

        public void Accept(IIRUnitVisitor visitor)
        {
            foreach (var unit in Units)
            {
                unit.Accept(visitor);
            }
        }
    }
}
