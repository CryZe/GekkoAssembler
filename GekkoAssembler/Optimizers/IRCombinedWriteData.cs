using System;
using System.Linq;

namespace GekkoAssembler
{
    public class IRCombinedWriteData : IRWriteData
    {
        public override int Address { get; }

        public override byte[] Data { get; }

        public IRCombinedWriteData(IRWriteData a, IRWriteData b)
        {
            Address = Math.Min(a.Address, b.Address);
            if (a.Address < b.Address)
            {
                Data = a.Data.Concat(b.Data).ToArray();
            }
            else
            {
                Data = b.Data.Concat(a.Data).ToArray();
            }
        }
    }
}
