using System;
using System.Collections.Generic;

namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRCodeBlock : IIRUnit
    {
        public IList<IIRUnit> Units { get; }

        public IRCodeBlock(IEnumerable<IIRUnit> units)
        {
            Units = new List<IIRUnit>(units).AsReadOnly();
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
