using System;
using GekkoAssembler.IntermediateRepresentation;

namespace GekkoAssembler.Optimizers
{
    public class ActionReplayOptimizer : IOptimizer
    {
        private readonly IOptimizer generalOptimizer;
        private readonly CodeBuilder builder;

        public ActionReplayOptimizer(CodeBuilder builder)
        {
            generalOptimizer = new GeneralOptimizer();
            this.builder = builder;
        }

        public IRCodeBlock Optimize(IRCodeBlock block)
        {
            block = block.Units
                .Replace<IRUnsigned8BitSet>(replaceUnsigned8BitSet)
                .Replace<IRUnsigned16BitSet>(replaceUnsigned16BitSet)
                .Replace<IRUnsigned32BitSet>(replaceUnsigned32BitSet)
                .Replace<IRUnsigned8BitUnset>(replaceUnsigned8BitUnset)
                .Replace<IRUnsigned16BitUnset>(replaceUnsigned16BitUnset)
                .Replace<IRUnsigned32BitUnset>(replaceUnsigned32BitUnset)
                .ToCodeBlock();

            return generalOptimizer.Optimize(block);
        }

        private IIRUnit replaceUnsigned8BitSet(IRUnsigned8BitSet instruction)
        {
            builder.AddWarning("There is no way of setting individual bits, using \"Unsigned 8 Write\" instead.");
            return new CustomIRWriteData(instruction.Address, new[] { instruction.Value });
        }

        private IIRUnit replaceUnsigned16BitSet(IRUnsigned16BitSet instruction)
        {
            builder.AddWarning("There is no way of setting individual bits, using \"Unsigned 16 Write\" instead.");
            return new CustomIRWriteData(instruction.Address, BitConverter.GetBytes(instruction.Value).SwapEndian16());
        }

        private IIRUnit replaceUnsigned32BitSet(IRUnsigned32BitSet instruction)
        {
            builder.AddWarning("There is no way of setting individual bits, using \"Unsigned 32 Write\" instead.");
            return new CustomIRWriteData(instruction.Address, BitConverter.GetBytes(instruction.Value).SwapEndian32());
        }

        private IIRUnit replaceUnsigned8BitUnset(IRUnsigned8BitUnset instruction)
        {
            builder.AddWarning("There is no way of unsetting individual bits, using \"Unsigned 8 Write\" instead.");
            return new CustomIRWriteData(instruction.Address, new byte[] { 0 });
        }

        private IIRUnit replaceUnsigned16BitUnset(IRUnsigned16BitUnset instruction)
        {
            builder.AddWarning("There is no way of unsetting individual bits, using \"Unsigned 16 Write\" instead.");
            return new CustomIRWriteData(instruction.Address, new byte[] { 0, 0 });
        }

        private IIRUnit replaceUnsigned32BitUnset(IRUnsigned32BitUnset instruction)
        {
            builder.AddWarning("There is no way of unsetting individual bits, using \"Unsigned 32 Write\" instead.");
            return new CustomIRWriteData(instruction.Address, new byte[] { 0, 0, 0, 0 });
        }
    }
}
