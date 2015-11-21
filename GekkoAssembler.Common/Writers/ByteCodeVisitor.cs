using System;
using System.Collections.Generic;
using System.Linq;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Writers
{
    public class ByteCodeVisitor : IIRUnitVisitor
    {
        public CodeBuilder Builder { get; }

        public ByteCodeVisitor(CodeBuilder builder)
        {
            Builder = builder;
        }

        public void Visit(IRWriteData instruction)
        {
            Builder.WriteLine($"{instruction.Address:X8}:");
            Builder.WriteLine("");
            var i = 0;
            foreach (var b in instruction.Data)
            {
                if (i % 8 == 0)
                {
                    Builder.WriteLine("");
                }

                Builder.Lines[Builder.Lines.Count - 1] += $"{b:X2}";
                i++;
            }
            Builder.WriteLine("");
        }

        public void Visit(IRCodeBlock block)
        {
            block.Accept(this);
        }

        #region Mask

        public void Visit(IRUnsigned8Mask instruction)
        {
        }

        public void Visit(IRUnsigned16Mask instruction)
        {
        }

        #endregion

        #region Equal

        public void Visit(IRUnsigned8Equal instruction)
        {
        }

        public void Visit(IRUnsigned16Equal instruction)
        {
        }

        public void Visit(IRUnsigned32Equal instruction)
        {
        }

        public void Visit(IRSigned8Equal instruction)
        {
        }

        public void Visit(IRSigned16Equal instruction)
        {
        }

        public void Visit(IRSigned32Equal instruction)
        {
        }

        public void Visit(IRFloat32Equal instruction)
        {
        }

        #endregion

        #region Unequal

        public void Visit(IRUnsigned8Unequal instruction)
        {
        }

        public void Visit(IRUnsigned16Unequal instruction)
        {
        }

        public void Visit(IRUnsigned32Unequal instruction)
        {
        }

        public void Visit(IRSigned8Unequal instruction)
        {
        }

        public void Visit(IRSigned16Unequal instruction)
        {
        }

        public void Visit(IRSigned32Unequal instruction)
        {
        }

        public void Visit(IRFloat32Unequal instruction)
        {
        }

        #endregion

        #region Less Than

        public void Visit(IRUnsigned8LessThan instruction)
        {
        }

        public void Visit(IRUnsigned16LessThan instruction)
        {
        }

        public void Visit(IRUnsigned32LessThan instruction)
        {
        }

        public void Visit(IRSigned8LessThan instruction)
        {
        }

        public void Visit(IRSigned16LessThan instruction)
        {
        }

        public void Visit(IRSigned32LessThan instruction)
        {
        }

        public void Visit(IRFloat32LessThan instruction)
        {
        }

        #endregion

        #region Greater Than

        public void Visit(IRUnsigned8GreaterThan instruction)
        {
        }

        public void Visit(IRUnsigned16GreaterThan instruction)
        {
        }

        public void Visit(IRUnsigned32GreaterThan instruction)
        {
        }

        public void Visit(IRSigned8GreaterThan instruction)
        {
        }

        public void Visit(IRSigned16GreaterThan instruction)
        {
        }

        public void Visit(IRSigned32GreaterThan instruction)
        {
        }

        public void Visit(IRFloat32GreaterThan instruction)
        {
        }

        #endregion

        #region Add

        public void Visit(IRUnsigned8Add instruction)
        {
        }

        public void Visit(IRUnsigned16Add instruction)
        {
        }

        public void Visit(IRUnsigned32Add instruction)
        {
        }

        public void Visit(IRSigned8Add instruction)
        {
        }

        public void Visit(IRSigned16Add instruction)
        {
        }

        public void Visit(IRSigned32Add instruction)
        {
        }

        public void Visit(IRFloat32Add instruction)
        {
        }

        #endregion

        #region Setting Bits

        public void Visit(IRUnsigned8BitSet instruction)
        {
        }

        public void Visit(IRUnsigned16BitSet instruction)
        {
        }

        public void Visit(IRUnsigned32BitSet instruction)
        {
        }

        #endregion

        #region Unsetting Bits

        public void Visit(IRUnsigned8BitUnset instruction)
        {
        }

        public void Visit(IRUnsigned16BitUnset instruction)
        {
        }

        public void Visit(IRUnsigned32BitUnset instruction)
        {
        }

        #endregion
    }
}
