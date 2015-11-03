using System;
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
                        Builder.WriteLine($"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {patternCount:X6}{valueI:X2}");
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
                        Builder.WriteLine($"{0x02 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {patternCount:X4}{valueI << 8 | valueI2:X4}");
                        i += 2 * patternCount;
                        continue;
                    }
                }

                Builder.WriteLine($"{0x04 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 24 | instruction.Data[i + 1] << 16 | instruction.Data[i + 2] << 8 | instruction.Data[i + 3]:X8}");
                i += 4;
            }

            if (i + 3 <= instruction.Data.Length && instruction.Data[i] == instruction.Data[i + 1] && instruction.Data[i] == instruction.Data[i + 2])
            {
                //3 times the same byte can be optimized into a single write
                Builder.WriteLine($"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} 000003{instruction.Data[i]:X2}");
                i += 3;
            }

            if (i + 2 <= instruction.Data.Length)
            {
                Builder.WriteLine($"{0x02 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 8 | instruction.Data[i + 1]:X8}");
                i += 2;
            }

            if (i < instruction.Data.Length)
            {
                Builder.WriteLine($"{0x00 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i]:X8}");
            }
        }

        public void Visit(IRCodeBlock block)
        {
            block.Accept(this);
        }

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
            //Note: There's no way of representing Floating Point Less Than. Using Signed 32 Bit Less Than instead.
            WriteActivator(instruction.ConditionalCode, 0x1C, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
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
    }
}
