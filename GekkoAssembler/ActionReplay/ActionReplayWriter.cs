using System.IO;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.ActionReplay
{
    public class ActionReplayWriter : IIRUnitVisitor
    {
        public Stream Stream { get; }
        private readonly StreamWriter writer;

        public ActionReplayWriter(Stream stream)
        {
            Stream = stream;
            writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public void Visit(IRWriteData instruction)
        {
            var i = 0;

            while ((i + 4) <= instruction.Data.Length)
            {
                writer.WriteLine(string.Format("04{0:X6} {1:X2}{2:X2}{3:X2}{4:X2}", ((instruction.Address + i) & 0xFFFFFF), instruction.Data[i], instruction.Data[i + 1], instruction.Data[i + 2], instruction.Data[i + 3]));
                i += 4;
            }

            if ((i + 2) <= instruction.Data.Length)
            {
                writer.WriteLine(string.Format("02{0:X6} 0000{1:X2}{2:X2}", ((instruction.Address + i) & 0xFFFFFF), instruction.Data[i], instruction.Data[i + 1]));
                i += 2;
            }

            if (i < instruction.Data.Length)
            {
                writer.WriteLine(string.Format("00{0:X6} 000000{1:X2}", ((instruction.Address + i) & 0xFFFFFF), instruction.Data[i]));
            }
        }
    }
}
