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
            WriteActivator(instruction.ConditionalCode, 0x08, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned16Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x0A, instruction.Address, instruction.Value);
        }

        private void WriteActivator(IRCodeBlock block, int type, int address, int value)
        {
            var lines = GetCodeBlockLines(block);

            if (lines.Count == 0)
            { }
            else if (lines.Count == 1)
            {
                writer.WriteLine($"{type << 24 | address & 0x1FFFFFF:X8} {value:X8}");
                foreach (var line in lines)
                    writer.WriteLine(line);
            }
            else if (lines.Count == 2)
            {
                writer.WriteLine($"{(type | 0x40) << 24 | address & 0x1FFFFFF:X8} {value:X8}");
                foreach (var line in lines)
                    writer.WriteLine(line);
            }
            else
            {
                writer.WriteLine($"{(type | 0x80) << 24 | address & 0x1FFFFFF:X8} {value:X8}");
                foreach (var line in lines)
                    writer.WriteLine(line);
                writer.WriteLine("00000000 40000000");
            }
        }

        private static List<string> GetCodeBlockLines(IRCodeBlock block)
        {
            using (var stream = new MemoryStream())
            {
                var innerWriter = new ActionReplayWriter(stream);
                block.Accept(innerWriter);
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);

                var lines = new List<string>();
                string _line;
                while ((_line = reader.ReadLine()) != null)
                    lines.Add(_line);

                return lines;
            }
        }
    }
}
