using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
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

            while (i + 4 <= instruction.Data.Length)
            {
                if (i + 5 <= instruction.Data.Length)
                {
                    //Check for reoccuring single byte patterns
                    var valueI = instruction.Data[i];

                    var patternCount = instruction.Data.Skip(i + 1).TakeWhile(x => x == valueI).Count() + 1;

                    if (patternCount > 4)
                    {
                        //Single Byte Pattern of more than 4 bytes found
                        writer.WriteLine($"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {patternCount:X6}{valueI:X2}");
                        i += patternCount;
                        continue;
                    }

                    //Check for reoccuring two byte patterns
                    var valueI2 = instruction.Data[i + 1];
                    var patternCountI1 = instruction.Data.Skip(i + 2).Where((x, id) => id % 2 == 0).TakeWhile(x => x == valueI).Count() + 1;
                    var patternCountI2 = instruction.Data.Skip(i + 2).Where((x, id) => id % 2 == 1).TakeWhile(x => x == valueI2).Count() + 1;
                    patternCount = Math.Min(patternCountI1, patternCountI2);

                    if (patternCount > 2)
                    {
                        writer.WriteLine($"{0x02 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {patternCount:X4}{valueI << 8 | valueI2:X4}");
                        i += 2 * patternCount;
                        continue;
                    }
                }

                writer.WriteLine($"{0x04 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 24 | instruction.Data[i + 1] << 16 | instruction.Data[i + 2] << 8 | instruction.Data[i + 3]:X8}");
                i += 4;
            }

            if (i + 3 <= instruction.Data.Length && instruction.Data[i] == instruction.Data[i + 1] && instruction.Data[i] == instruction.Data[i + 2])
            {
                //3 times the same byte can be optimized into a single write
                writer.WriteLine($"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} 000003{instruction.Data[i]:X2}");
                i += 3;
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

        public void Visit(IRUnsigned32Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x0C, instruction.Address, (int)instruction.Value);
        }

        public void Visit(IRSigned8Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x08, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned16Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x0A, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x0C, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x0C, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
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

        public void Visit(IRUnsigned8Add instruction)
        {
            writer.WriteLine($"{0x80 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRUnsigned16Add instruction)
        {
            writer.WriteLine($"{0x82 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRUnsigned32Add instruction)
        {
            writer.WriteLine($"{0x84 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRFloat32Add instruction)
        {
            writer.WriteLine($"{0x86 << 24 | instruction.Address & 0x1FFFFFF:X8} {BitConverter.ToUInt32(BitConverter.GetBytes(instruction.Value), 0):X8}");
        }
    }
}
