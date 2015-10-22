using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GekkoAssembler
{
    public class CombinedDataSection : IGekkoDataSection
    {
        public int Address { get; }

        public byte[] Data { get; }

        public CombinedDataSection(IGekkoDataSection a, IGekkoDataSection b)
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
