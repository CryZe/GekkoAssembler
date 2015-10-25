using System;

namespace GekkoAssembler.DataSections
{
    public class Signed8DataSection : GekkoDataSection
    {
        public override int Address { get; }
        public override byte[] Data => BitConverter.GetBytes(Value);
        public sbyte Value { get; }

        public Signed8DataSection(int address, sbyte value)
        {
            Address = address;
            Value = value;
        }
    }
}
