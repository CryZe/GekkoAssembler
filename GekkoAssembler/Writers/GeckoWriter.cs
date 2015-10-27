using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
{
    public class GeckoWriter : IIRUnitVisitor
    {
        public Stream Stream { get; }
        private readonly StreamWriter writer;

        public GeckoWriter(Stream stream)
        {
            Stream = stream;
            writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public void Visit(IRWriteData instruction)
        {
            var i = 0;

            if (instruction.Data.Length <= 4)
            {
                if (i + 4 == instruction.Data.Length)
                {
                    writer.WriteLine($"{0x04 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 24 | instruction.Data[i + 1] << 16 | instruction.Data[i + 2] << 8 | instruction.Data[i + 3]:X8}");
                    i += 4;
                }

                if (i + 2 <= instruction.Data.Length)
                {
                    writer.WriteLine($"{0x02 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 8 | instruction.Data[i + 1]:X8}");
                    i += 2;
                }

                if (i < instruction.Data.Length)
                {
                    writer.WriteLine($"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i]:X8}");
                }
            }
            else
            {
                writer.WriteLine($"{0x06 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data.Length:X8}");

                while (i < instruction.Data.Length)
                {
                    var bytes = instruction.Data.Skip(i).Take(8).ToList();
                    bytes.AddRange(Enumerable.Repeat<byte>(0, 8 - bytes.Count()));
                    var first = BitConverter.ToUInt32(bytes.Take(4).ToArray().SwapEndian32(), 0);
                    var second = BitConverter.ToUInt32(bytes.Skip(4).Take(4).ToArray().SwapEndian32(), 0);
                    writer.WriteLine($"{first:X8} {second:X8}");
                    i += 8;
                }
            }            
        }

        public void Visit(IRCodeBlock block)
        {
            block.Accept(this);
        }

        public void Visit(IRUnsigned8Equal instruction)
        {
            writer.WriteLine($"82000000 {instruction.Address:X8}");
            writer.WriteLine($"80000001 {instruction.Value:X8}");
            writer.WriteLine($"A0000000 0100FF00");

            var lines = GetCodeBlockLines(instruction.ConditionalCode);

            foreach (var line in lines)
                writer.WriteLine(line);

            writer.WriteLine("E2000001 00000000");
        }

        public void Visit(IRUnsigned16Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x28, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x20, instruction.Address, (int)instruction.Value);
        }

        public void Visit(IRSigned8Equal instruction)
        {
            writer.WriteLine($"82000000 {instruction.Address:X8}");
            writer.WriteLine($"80000001 {instruction.Value:X8}");
            writer.WriteLine($"A0000000 0100FF00");

            var lines = GetCodeBlockLines(instruction.ConditionalCode);

            foreach (var line in lines)
                writer.WriteLine(line);

            writer.WriteLine("E2000001 00000000");
        }

        public void Visit(IRSigned16Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x28, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x20, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x20, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
        }

        private void WriteActivator(IRCodeBlock block, int type, int address, int value)
        {
            var lines = GetCodeBlockLines(block);

            if (lines.Count > 0)
            {
                writer.WriteLine($"{type << 24 | address & 0x1FFFFFF:X8} {value:X8}");
                foreach (var line in lines)
                    writer.WriteLine(line);
                writer.WriteLine("E2000001 00000000");
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

        public void Visit(IRUnsigned8Add instruction)
        {
            writer.WriteLine($"82000000 {instruction.Address:X8}");
            writer.WriteLine($"86000000 {instruction.Value:X8}");
            writer.WriteLine($"84000000 {instruction.Address:X8}");
        }

        public void Visit(IRUnsigned16Add instruction)
        {
            writer.WriteLine($"82100000 {instruction.Address:X8}");
            writer.WriteLine($"86000000 {instruction.Value:X8}");
            writer.WriteLine($"84100000 {instruction.Address:X8}");
        }

        public void Visit(IRUnsigned32Add instruction)
        {
            writer.WriteLine($"82200000 {instruction.Address:X8}");
            writer.WriteLine($"86000000 {instruction.Value:X8}");
            writer.WriteLine($"84200000 {instruction.Address:X8}");
        }

        public void Visit(IRFloat32Add instruction)
        {
            writer.WriteLine($"82200000 {instruction.Address:X8}");
            writer.WriteLine($"86900000 {BitConverter.ToUInt32(BitConverter.GetBytes(instruction.Value), 0):X8}");
            writer.WriteLine($"84200000 {instruction.Address:X8}");
        }
    }
}
