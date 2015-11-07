using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.DataSections
{
    public abstract class GekkoDataSection<T> : IRWriteData
    {
        public override int Address { get; }

        protected T Value { get; }

        protected GekkoDataSection(int address, T value)
        {
            this.Value = value;
            this.Address = address;
        }
    }
}
