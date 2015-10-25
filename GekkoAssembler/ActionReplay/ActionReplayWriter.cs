using System.Collections.Generic;
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
                writer.WriteLine($"{0x04 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 24 | instruction.Data[i + 1] << 16 | instruction.Data[i + 2] << 8 | instruction.Data[i + 3]:X8}");
                i += 4;
            }

            if ((i + 2) <= instruction.Data.Length)
            {
                writer.WriteLine($"{0x02 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 8 | instruction.Data[i + 1]:X8}");
                i += 2;
            }

            if (i < instruction.Data.Length)
            {
                writer.WriteLine($"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i]:X8}");
            }
        }

        public void Visit(IRCodeBlock block)
        {
            block.Accept(this);
        }

        public void Visit(IRUnsigned8Equal instruction)
        {
            using (var stream = new MemoryStream())
            {
                var innerWriter = new ActionReplayWriter(stream);
                instruction.ConditionalCode.Accept(innerWriter);
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);

                var lines = new List<string>();
                string _line;
                while ((_line = reader.ReadLine()) != null)
                    lines.Add(_line);

                if (lines.Count == 0)
                { }
                else if (lines.Count == 1)
                {
                    writer.WriteLine($"{0x08 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
                    foreach (var line in lines)
                        writer.WriteLine(line);
                }
                else if (lines.Count == 2)
                {
                    writer.WriteLine($"{0x48 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
                    foreach (var line in lines)
                        writer.WriteLine(line);
                }
                else
                {
                    writer.WriteLine($"{0x88 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
                    foreach (var line in lines)
                        writer.WriteLine(line);
                    writer.WriteLine("00000000 40000000");
                }
            }
        }
    }
}
