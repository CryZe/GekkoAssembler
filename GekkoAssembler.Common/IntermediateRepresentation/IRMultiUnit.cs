using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GekkoAssembler.IntermediateRepresentation
{
    public class IRMultiUnit : IIRUnit
    {
        public ReadOnlyCollection<IIRUnit> Inner { get; }

        public IRMultiUnit(IEnumerable<IIRUnit> inner)
        {
            Inner = inner.AsReadOnly();
        }

        public void Accept(IIRUnitVisitor visitor)
        {
            foreach (var unit in Inner)
                unit.Accept(visitor);
        }
    }
}
