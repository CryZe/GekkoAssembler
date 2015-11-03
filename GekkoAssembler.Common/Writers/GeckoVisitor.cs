using System;
using System.Linq;
using GekkoAssembler.IntermediateRepresentation;
using System.Globalization;

namespace GekkoAssembler.Writers
{
    public class GeckoVisitor : IIRUnitVisitor
    {
        public CodeBuilder Builder { get; }

        public GeckoVisitor(CodeBuilder builder)
        {
            Builder = builder;
        }

        public void Visit(IRWriteData instruction)
        {
            var i = 0;

            if (instruction.Data.Length <= 4)
            {
                if (i + 4 == instruction.Data.Length)
                {
                    Builder.WriteLine($"{0x04 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data[i] << 24 | instruction.Data[i + 1] << 16 | instruction.Data[i + 2] << 8 | instruction.Data[i + 3]:X8}");
                    i += 4;
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
            else
            {
                Builder.WriteLine($"{0x06 << 24 | (instruction.Address + i) & 0x1FFFFFF:X8} {instruction.Data.Length:X8}");

                while (i < instruction.Data.Length)
                {
                    var bytes = instruction.Data.Skip(i).Take(8).ToList();
                    bytes.AddRange(Enumerable.Repeat<byte>(0, 8 - bytes.Count()));
                    var first = BitConverter.ToUInt32(bytes.Take(4).ToArray().SwapEndian32(), 0);
                    var second = BitConverter.ToUInt32(bytes.Skip(4).Take(4).ToArray().SwapEndian32(), 0);
                    Builder.WriteLine($"{first:X8} {second:X8}");
                    i += 8;
                }
            }            
        }

        public void Visit(IRCodeBlock block)
        {
            block.Accept(this);
        }

        #region Equal

        public void Visit(IRUnsigned8Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x28, instruction.Address, instruction.Value, 0xFF000000);
        }

        public void Visit(IRUnsigned16Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x28, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x20, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned8Equal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x28, instruction.Address, instruction.Value, 0xFF000000);
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

        #endregion

        #region Unequal

        public void Visit(IRUnsigned8Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2A, instruction.Address, instruction.Value, 0xFF000000);
        }

        public void Visit(IRUnsigned16Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2A, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x22, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned8Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2A, instruction.Address, instruction.Value, 0xFF000000);
        }

        public void Visit(IRSigned16Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2A, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x22, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32Unequal instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x22, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
        }

        #endregion

        #region Less Than

        public void Visit(IRUnsigned8LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2E, instruction.Address, instruction.Value, 0xFF000000);
        }

        public void Visit(IRUnsigned16LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2E, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32LessThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x26, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned8LessThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Signed 8 Bit Less Than\", using \"Unsigned 8 Bit Less Than\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x2E, instruction.Address, instruction.Value, 0xFF000000);
        }

        public void Visit(IRSigned16LessThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Signed 16 Bit Less Than\", using \"Unigned 16 Bit Less Than\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x2E, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32LessThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Signed 32 Bit Less Than\", using \"Unsigned 32 Bit Less Than\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x26, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32LessThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Floating Point Less Than\", using \"Unsigned 32 Bit Less Than\" instead. That's correct for positive floating point numbers.");
            WriteActivator(instruction.ConditionalCode, 0x22, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
        }

        #endregion

        #region Greater Than

        public void Visit(IRUnsigned8GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2C, instruction.Address, instruction.Value, 0xFF000000);
        }

        public void Visit(IRUnsigned16GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x2C, instruction.Address, instruction.Value);
        }

        public void Visit(IRUnsigned32GreaterThan instruction)
        {
            WriteActivator(instruction.ConditionalCode, 0x24, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned8GreaterThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Signed 8 Bit Greater Than\", using \"Unsigned 8 Bit Greater Than\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x2C, instruction.Address, instruction.Value, 0xFF000000);
        }

        public void Visit(IRSigned16GreaterThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Signed 16 Bit Greater Than\", using \"Unigned 16 Bit Greater Than\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x2C, instruction.Address, instruction.Value);
        }

        public void Visit(IRSigned32GreaterThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Signed 32 Bit Greater Than\", using \"Unsigned 32 Bit Greater Than\" instead.");
            WriteActivator(instruction.ConditionalCode, 0x24, instruction.Address, instruction.Value);
        }

        public void Visit(IRFloat32GreaterThan instruction)
        {
            Builder.AddWarning("There is no way of representing \"Floating Point Greater Than\", using \"Unsigned 32 Bit Greater Than\" instead. That's correct for positive floating point numbers.");
            WriteActivator(instruction.ConditionalCode, 0x24, instruction.Address, BitConverter.ToInt32(BitConverter.GetBytes(instruction.Value), 0));
        }

        #endregion

        private void WriteActivator<T>(IRCodeBlock block, int type, int address, T value, uint mask = 0)
        {
            var code = new GeckoWriter().WriteCode(block);

            Builder.Warnings.AddRange(code.Warnings);
            Builder.Errors.AddRange(code.Errors);

            if (code.Lines.Count > 0)
            {
                var masked = uint.Parse($"{value:X8}", NumberStyles.HexNumber, CultureInfo.InvariantCulture) | mask;
                Builder.WriteLine($"{ type << 24 | address & 0x1FFFFFF:X8} { masked:X8}");
                Builder.Lines.AddRange(code.Lines);
                Builder.WriteLine("E2000001 00000000");
            }
        }

        #region Add

        public void Visit(IRUnsigned8Add instruction)
        {
            Builder.WriteLine($"82000000 {instruction.Address:X8}");
            Builder.WriteLine($"86000000 {instruction.Value:X8}");
            Builder.WriteLine($"84000000 {instruction.Address:X8}");
        }

        public void Visit(IRUnsigned16Add instruction)
        {
            Builder.WriteLine($"82100000 {instruction.Address:X8}");
            Builder.WriteLine($"86000000 {instruction.Value:X8}");
            Builder.WriteLine($"84100000 {instruction.Address:X8}");
        }

        public void Visit(IRUnsigned32Add instruction)
        {
            Builder.WriteLine($"82200000 {instruction.Address:X8}");
            Builder.WriteLine($"86000000 {instruction.Value:X8}");
            Builder.WriteLine($"84200000 {instruction.Address:X8}");
        }

        public void Visit(IRSigned8Add instruction)
        {
            Builder.WriteLine($"82000000 {instruction.Address:X8}");
            Builder.WriteLine($"86000000 {instruction.Value:X8}");
            Builder.WriteLine($"84000000 {instruction.Address:X8}");
        }

        public void Visit(IRSigned16Add instruction)
        {
            Builder.WriteLine($"82100000 {instruction.Address:X8}");
            Builder.WriteLine($"86000000 {instruction.Value:X8}");
            Builder.WriteLine($"84100000 {instruction.Address:X8}");
        }

        public void Visit(IRSigned32Add instruction)
        {
            Builder.WriteLine($"82200000 {instruction.Address:X8}");
            Builder.WriteLine($"86000000 {instruction.Value:X8}");
            Builder.WriteLine($"84200000 {instruction.Address:X8}");
        }

        public void Visit(IRFloat32Add instruction)
        {
            Builder.WriteLine($"82200000 {instruction.Address:X8}");
            Builder.WriteLine($"86900000 {BitConverter.ToUInt32(BitConverter.GetBytes(instruction.Value), 0):X8}");
            Builder.WriteLine($"84200000 {instruction.Address:X8}");
        }

        #endregion
    }
}
