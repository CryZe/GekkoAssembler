using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRCodeBlock : IIRUnit
    {
        public ReadOnlyCollection<IIRUnit> Units { get; }

        public IRCodeBlock(IEnumerable<IIRUnit> units)
        {
            Units = units.AsReadOnly();
        }

        void IIRUnit.Accept(IIRUnitVisitor visitor)
        {
            visitor.Visit(this);
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
