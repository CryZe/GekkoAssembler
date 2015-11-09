using System;
using System.Collections.Generic;
using System.Linq;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
{
    public class ActionReplayVisitor : IIRUnitVisitor
    {
        public CodeBuilder Builder { get; }

        public ActionReplayVisitor(CodeBuilder builder)
        {
            Builder = builder;
        }

        public void Visit(IRWriteData instruction)
        {
            foreach (var line in GetWriteDataLines(instruction))
                Builder.WriteLine(line);
        }

        private static IEnumerable<string> GetWriteDataLines(IRWriteData instruction)
        {
            var i = 0;

            while (i + 4 <= instruction.Length)
            {
                if (i + 5 <= instruction.Length)
                {
                    //Check for reoccuring single byte patterns
                    var valueI = instruction.Data[i];

                    var patternCount = instruction.Data.Skip(i + 1).TakeWhile(x => x == valueI).Count() + 1;

                    var bytesLeft = instruction.Length - (i + patternCount);

                    if (bytesLeft >= 4)
                    {
                        //Keep Alignment
                        patternCount = 4 * (patternCount / 4);
                    }
                    else if (bytesLeft >= 2)
                    {
                        //Keep Alignment
                        patternCount = 2 * (patternCount / 2);
                    }

                    if (patternCount > 4)
                    {
                        //Single Byte Pattern of more than 4 bytes found
                        yield return $"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {patternCount - 1:X6}{valueI:X2}";
                        i += patternCount;
                        continue;
                    }

                    //Check for reoccuring two byte patterns
                    var valueI2 = instruction.Data[i + 1];
                    var patternCountI1 = instruction.Data.Skip(i + 2).Where((x, id) => id % 2 == 0).TakeWhile(x => x == valueI).Count() + 1;
                    var patternCountI2 = instruction.Data.Skip(i + 2).Where((x, id) => id % 2 == 1).TakeWhile(x => x == valueI2).Count() + 1;
                    patternCount = Math.Min(patternCountI1, patternCountI2);

                    bytesLeft = instruction.Length - (i + 2 * patternCount);

                    if (bytesLeft >= 4)
                    {
                        //Keep Alignment
                        patternCount = 2 * (patternCount / 2);
                    }

                    if (patternCount > 2)
                    {
                        yield return $"{0x02 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {patternCount - 1:X4}{valueI << 8 | valueI2:X4}";
                        i += 2 * patternCount;
                        continue;
                    }
                }

                yield return $"{0x04 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 24 | instruction.Data[i + 1] << 16 | instruction.Data[i + 2] << 8 | instruction.Data[i + 3]:X8}";
                i += 4;
            }

            if (i + 3 <= instruction.Length && instruction.Data[i] == instruction.Data[i + 1] && instruction.Data[i] == instruction.Data[i + 2])
            {
                //3 times the same byte can be optimized into a single write
                yield return $"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} 000003{instruction.Data[i]:X2}";
                i += 3;
            }

            if (i + 2 <= instruction.Length)
            {
                yield return $"{0x02 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 8 | instruction.Data[i + 1]:X8}";
                i += 2;
            }

            if (i < instruction.Length)
            {
                yield return $"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i]:X8}";
            }
        }

        public void Visit(IRCodeBlock block)
        {
            block.Accept(this);
        }

        #region Mask

        public void Visit(IRUnsigned8Mask instruction)
        {
            Builder.AddWarning("There is no way of representing Masks, using \"Unsigned 8 Equal\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x08, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned16Mask instruction)
        {
            Builder.AddWarning("There is no way of representing Masks, using \"Unsigned 16 Equal\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x0A, instruction.Address, instruction.Value);
        }

        #endregion

        #region Equal

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
            WriteActivator(instruction.ConditionalCode, 0x0C, instruction.Address, instruction.Value);
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

        #endregion

        #region Unequal

        public void Visit(IRUnsigned8Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x10, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned16Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x12, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x14, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned8Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x10, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned16Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x12, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x14, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x14, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
        }

        #endregion

        #region Less Than

        public void Visit(IRUnsigned8LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x28, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned16LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2A, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2C, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned8LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x18, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned16LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x1A, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x1C, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32LessThan instruction)
        {
            if (instruction.Value < 0)
                WriteActivator(instruction.ConditionalCode, 0x34, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
            else
                WriteActivator(instruction.ConditionalCode, 0x1C, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
        }

        #endregion

        #region Greater Than

        public void Visit(IRUnsigned8GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x30, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned16GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x32, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x34, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned8GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x20, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned16GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x22, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x24, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32GreaterThan instruction)
        {
            if (instruction.Value < 0)
                WriteActivator(instruction.ConditionalCode, 0x2C, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
            else
                WriteActivator(instruction.ConditionalCode, 0x24, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
        }

        #endregion

        private void WriteActivator<T>(IRCodeBlock block, int type, int address, T value)
        {
            var code = new ActionReplayWriter().WriteCode(block);

            Builder.Warnings.AddRange(code.Warnings);
            Builder.Errors.AddRange(code.Errors);

            var lines = code.Lines;

            if (lines.Count == 0)
            { }
            else if (lines.Count == 1)
            {
                Builder.WriteLine($"{type << 24 | address & 0x1FFFFFF:X8} {value:X8}");
                Builder.Lines.AddRange(lines);
            }
            else if (lines.Count == 2)
            {
                Builder.WriteLine($"{(type | 0x40) << 24 | address & 0x1FFFFFF:X8} {value:X8}");
                Builder.Lines.AddRange(lines);
            }
            else
            {
                Builder.WriteLine($"{(type | 0x80) << 24 | address & 0x1FFFFFF:X8} {value:X8}");
                Builder.Lines.AddRange(lines);
                Builder.WriteLine("00000000 40000000");
            }
        }

        #region Add

        public void Visit(IRUnsigned8Add instruction)
        {
            Builder.WriteLine($"{0x80 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRUnsigned16Add instruction)
        {
            Builder.WriteLine($"{0x82 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRUnsigned32Add instruction)
        {
            Builder.WriteLine($"{0x84 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRSigned8Add instruction)
        {
            Builder.WriteLine($"{0x80 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRSigned16Add instruction)
        {
            Builder.WriteLine($"{0x82 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRSigned32Add instruction)
        {
            Builder.WriteLine($"{0x84 << 24 | instruction.Address & 0x1FFFFFF:X8} {instruction.Value:X8}");
        }

        public void Visit(IRFloat32Add instruction)
        {
            Builder.WriteLine($"{0x86 << 24 | instruction.Address & 0x1FFFFFF:X8} {BitConverter.ToUInt32(BitConverter.GetBytes(instruction.Value), 0):X8}");
        }

        #endregion

        #region Bit Set

        public void Visit(IRUnsigned8BitSet instruction)
        {
            Builder.AddWarning("There is no way of representing Bit Sets, using \"Unsigned 8 Write\" instead.");
            var dataSection = new CustomIRWriteData(instruction.Address, new[] { instruction.Value });
            Visit(dataSection);
        }

        public void Visit(IRUnsigned16BitSet instruction)
        {
            Builder.AddWarning("There is no way of representing Bit Sets, using \"Unsigned 16 Write\" instead.");
            var dataSection = new CustomIRWriteData(instruction.Address, BitConverter.GetBytes(instruction.Value).SwapEndian16());
            Visit(dataSection);
        }

        public void Visit(IRUnsigned32BitSet instruction)
        {
            Builder.AddWarning("There is no way of representing Bit Sets, using \"Unsigned 32 Write\" instead.");
            var dataSection = new CustomIRWriteData(instruction.Address, BitConverter.GetBytes(instruction.Value).SwapEndian32());
            Visit(dataSection);
        }

        #endregion
    }
}
