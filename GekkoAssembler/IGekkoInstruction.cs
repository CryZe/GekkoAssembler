﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GekkoAssembler
{
    public interface IGekkoInstruction
    {
        int Address { get; }
        int ByteCode { get; }
    }

    public static class IGekkoInstructionExtensions
    {
        public static string ToCheat(this IGekkoInstruction instruction)
        {
            return string.Format("04{0:X6} {1:X8}", (instruction.Address & 0xFFFFFF), (uint)instruction.ByteCode);
        }

        public static string ToCheat(this IEnumerable<IGekkoInstruction> instructionList)
        {
            return string.Join(Environment.NewLine, instructionList.Select(x => x.ToCheat()));
        }
    }
}
