using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GekkoAssembler.DataSections;
using GekkoAssembler.GekkoInstructions;
using GekkoAssembler.IntermediateRepresentation;
using GekkoAssembler.Optimizers;

namespace GekkoAssembler
{
    public class Assembler
    {
        private static readonly Dictionary<string, Func<string[], int, GekkoInstruction>> instructionTable = new Dictionary<string, Func<string[], int, GekkoInstruction>>
        {
            {"add"        , ParseInstructionADD              },
            {"add."       , ParseInstructionADD              },
            {"addo"       , ParseInstructionADD              },
            {"addo."      , ParseInstructionADD              },
            {"addc"       , ParseInstructionADDC             },
            {"addc."      , ParseInstructionADDC             },
            {"addco"      , ParseInstructionADDC             },
            {"addco."     , ParseInstructionADDC             },
            {"adde"       , ParseInstructionADDE             },
            {"adde."      , ParseInstructionADDE             },
            {"addeo"      , ParseInstructionADDE             },
            {"addeo."     , ParseInstructionADDE             },
            {"addi"       , ParseInstructionADDI             },
            {"addic"      , ParseInstructionADDIC            },
            {"addic."     , ParseInstructionADDIC            },
            {"addis"      , ParseInstructionADDIS            },
            {"addme"      , ParseInstructionADDME            },
            {"addme."     , ParseInstructionADDME            },
            {"addmeo"     , ParseInstructionADDME            },
            {"addmeo."    , ParseInstructionADDME            },
            {"addze"      , ParseInstructionADDZE            },
            {"addze."     , ParseInstructionADDZE            },
            {"addzeo"     , ParseInstructionADDZE            },
            {"addzeo."    , ParseInstructionADDZE            },
            {"and"        , ParseInstructionAND              },
            {"and."       , ParseInstructionAND              },
            {"andc"       , ParseInstructionAND              },
            {"andc."      , ParseInstructionAND              },
            {"andi."      , ParseIntegerLogicalImmediate     },
            {"andis."     , ParseIntegerLogicalImmediate     },
            {"b"          , ParseInstructionB                },
            {"ba"         , ParseInstructionBA               },
            {"bl"         , ParseInstructionBL               },
            {"bla"        , ParseInstructionBLA              },
            {"blr"        , ParseInstructionBLR              },
            {"cmp"        , ParseInstructionCMP              },
            {"cmpi"       , ParseInstructionCMP              },
            {"cmpl"       , ParseInstructionCMP              },
            {"cmpli"      , ParseInstructionCMP              },
            {"cmplw"      , ParseInstructionCMP              },
            {"cmplwi"     , ParseInstructionCMP              },
            {"cmpw"       , ParseInstructionCMP              },
            {"cmpwi"      , ParseInstructionCMP              },
            {"cntlzw"     , ParseInstructionCNTLZW           },
            {"cntlzw."    , ParseInstructionCNTLZW           },
            {"crand"      , ParseInstructionCRAND            },
            {"crandc"     , ParseInstructionCRANDC           },
            {"crclr"      , ParseInstructionCRCLR            },
            {"creqv"      , ParseInstructionCREQV            },
            {"crmove"     , ParseInstructionCRMOVE           },
            {"crnand"     , ParseInstructionCRNAND           },
            {"crnor"      , ParseInstructionCRNOR            },
            {"crnot"      , ParseInstructionCRNOT            },
            {"cror"       , ParseInstructionCROR             },
            {"crorc"      , ParseInstructionCRORC            },
            {"crset"      , ParseInstructionCRSET            },
            {"crxor"      , ParseInstructionCRXOR            },
            {"dcbf"       , ParseInstructionDataCache        },
            {"dcbi"       , ParseInstructionDataCache        },
            {"dcbst"      , ParseInstructionDataCache        },
            {"dcbt"       , ParseInstructionDataCache        },
            {"dcbtst"     , ParseInstructionDataCache        },
            {"dcbz"       , ParseInstructionDataCache        },
            {"dcbz_l"     , ParseInstructionDataCache        },
            {"divw"       , ParseInstructionDIVW             },
            {"divw."      , ParseInstructionDIVW             },
            {"divwo"      , ParseInstructionDIVW             },
            {"divwo."     , ParseInstructionDIVW             },
            {"divwu"      , ParseInstructionDIVW             },
            {"divwu."     , ParseInstructionDIVW             },
            {"divwuo"     , ParseInstructionDIVW             },
            {"divwuo."    , ParseInstructionDIVW             },
            {"eciwx"      , ParseInstructionExternalControl  },
            {"ecowx"      , ParseInstructionExternalControl  },
            {"eieio"      , ParseInstructionEIEIO            },
            {"eqv"        , ParseInstructionEQV              },
            {"eqv."       , ParseInstructionEQV              },
            {"extsb"      , ParseInstructionSignExtension    },
            {"extsb."     , ParseInstructionSignExtension    },
            {"extsh"      , ParseInstructionSignExtension    },
            {"extsh."     , ParseInstructionSignExtension    },
            {"fabs"       , ParseFloatingPointSingleOperand  },
            {"fabs."      , ParseFloatingPointSingleOperand  },
            {"fadd"       , ParseFloatingPointTwoOperand     },
            {"fadd."      , ParseFloatingPointTwoOperand     },
            {"fadds"      , ParseFloatingPointTwoOperand     },
            {"fadds."     , ParseFloatingPointTwoOperand     },
            {"fcmpo"      , ParseFloatingPointCompare        },
            {"fcmpu"      , ParseFloatingPointCompare        },
            {"fctiw"      , ParseFloatingPointSingleOperand  },
            {"fctiw."     , ParseFloatingPointSingleOperand  },
            {"fctiwz"     , ParseFloatingPointSingleOperand  },
            {"fctiwz."    , ParseFloatingPointSingleOperand  },
            {"fdiv"       , ParseFloatingPointTwoOperand     },
            {"fdiv."      , ParseFloatingPointTwoOperand     },
            {"fdivs"      , ParseFloatingPointTwoOperand     },
            {"fdivs."     , ParseFloatingPointTwoOperand     },
            {"fmadd"      , ParseFloatingPointThreeOperand   },
            {"fmadd."     , ParseFloatingPointThreeOperand   },
            {"fmadds"     , ParseFloatingPointThreeOperand   },
            {"fmadds."    , ParseFloatingPointThreeOperand   },
            {"fmr"        , ParseFloatingPointSingleOperand  },
            {"fmr."       , ParseFloatingPointSingleOperand  },
            {"fmsub"      , ParseFloatingPointThreeOperand   },
            {"fmsub."     , ParseFloatingPointThreeOperand   },
            {"fmsubs"     , ParseFloatingPointThreeOperand   },
            {"fmsubs."    , ParseFloatingPointThreeOperand   },
            {"fmul"       , ParseFloatingPointMultiply       },
            {"fmul."      , ParseFloatingPointMultiply       },
            {"fmuls"      , ParseFloatingPointMultiply       },
            {"fmuls."     , ParseFloatingPointMultiply       },
            {"fnabs"      , ParseFloatingPointSingleOperand  },
            {"fnabs."     , ParseFloatingPointSingleOperand  },
            {"fneg"       , ParseFloatingPointSingleOperand  },
            {"fneg."      , ParseFloatingPointSingleOperand  },
            {"fnmadd"     , ParseFloatingPointThreeOperand   },
            {"fnmadd."    , ParseFloatingPointThreeOperand   },
            {"fnmadds"    , ParseFloatingPointThreeOperand   },
            {"fnmadds."   , ParseFloatingPointThreeOperand   },
            {"fnmsub"     , ParseFloatingPointThreeOperand   },
            {"fnmsub."    , ParseFloatingPointThreeOperand   },
            {"fnmsubs"    , ParseFloatingPointThreeOperand   },
            {"fnmsubs."   , ParseFloatingPointThreeOperand   },
            {"fres"       , ParseFloatingPointSingleOperand  },
            {"fres."      , ParseFloatingPointSingleOperand  },
            {"frsp"       , ParseFloatingPointSingleOperand  },
            {"frsp."      , ParseFloatingPointSingleOperand  },
            {"frsqrte"    , ParseFloatingPointSingleOperand  },
            {"frsqrte."   , ParseFloatingPointSingleOperand  },
            {"fsel"       , ParseFloatingPointThreeOperand   },
            {"fsel."      , ParseFloatingPointThreeOperand   },
            {"fsub"       , ParseFloatingPointTwoOperand     },
            {"fsub."      , ParseFloatingPointTwoOperand     },
            {"fsubs"      , ParseFloatingPointTwoOperand     },
            {"fsubs."     , ParseFloatingPointTwoOperand     },
            {"icbi"       , ParseInstructionICBI             },
            {"isync"      , ParseInstructionISYNC            },
            {"lbz"        , ParseInstructionLoadInteger      },
            {"lbzu"       , ParseInstructionLoadInteger      },
            {"lbzux"      , ParseInstructionLoadInteger      },
            {"lbzx"       , ParseInstructionLoadInteger      },
            {"lfd"        , ParseLoadFloatingPoint           },
            {"lfdu"       , ParseLoadFloatingPoint           },
            {"lfdux"      , ParseLoadFloatingPointIndexed    },
            {"lfdx"       , ParseLoadFloatingPointIndexed    },
            {"lfs"        , ParseLoadFloatingPoint           },
            {"lfsu"       , ParseLoadFloatingPoint           },
            {"lfsux"      , ParseLoadFloatingPointIndexed    },
            {"lfsx"       , ParseLoadFloatingPointIndexed    },
            {"lha"        , ParseInstructionLoadInteger      },
            {"lhau"       , ParseInstructionLoadInteger      },
            {"lhaux"      , ParseInstructionLoadInteger      },
            {"lhax"       , ParseInstructionLoadInteger      },
            {"lhbrx"      , ParseInstructionLoadInteger      },
            {"lhz"        , ParseInstructionLoadInteger      },
            {"lhzu"       , ParseInstructionLoadInteger      },
            {"lhzux"      , ParseInstructionLoadInteger      },
            {"lhzx"       , ParseInstructionLoadInteger      },
            {"lis"        , ParseInstructionLIS              },
            {"lwarx"      , ParseInstructionLoadInteger      },
            {"lwbrx"      , ParseInstructionLoadInteger      },
            {"lwz"        , ParseInstructionLoadInteger      },
            {"lwzu"       , ParseInstructionLoadInteger      },
            {"lwzux"      , ParseInstructionLoadInteger      },
            {"lwzx"       , ParseInstructionLoadInteger      },
            {"mcrf"       , ParseInstructionMoveToCR         },
            {"mcrfs"      , ParseInstructionMoveToCR         },
            {"mcrxr"      , ParseInstructionMoveToCR         },
            {"mfcr"       , ParseInstructionMFCR             },
            {"mffs"       , ParseInstructionMFFS             },
            {"mffs."      , ParseInstructionMFFS             },
            {"mflr"       , ParseInstructionMFLR             },
            {"mfmsr"      , ParseInstructionMFMSR            },
            {"mfsr"       , ParseInstructionMFSR             },
            {"mfsrin"     , ParseInstructionMFSRIN           },
            {"mfspr"      , ParseInstructionMFSPR            },
            {"mftb"       , ParseInstructionMFTB             },
            {"mftbu"      , ParseInstructionMFTB             },
            {"mtlr"       , ParseInstructionMTLR             },
            {"mtspr"      , ParseInstructionMTSPR            },
            {"mulli"      , ParseInstructionMULLI            },
            {"mullw"      , ParseInstructionMULLW            },
            {"mullw."     , ParseInstructionMULLW            },
            {"mullwo"     , ParseInstructionMULLW            },
            {"mullwo."    , ParseInstructionMULLW            },
            {"nop"        , ParseInstructionNOP              },
            {"ori"        , ParseIntegerLogicalImmediate     },
            {"oris"       , ParseIntegerLogicalImmediate     },
            {"psq_l"      , ParsePairedSingleLoadStore       },
            {"psq_lu"     , ParsePairedSingleLoadStore       },
            {"psq_lux"    , ParsePairedSingleLoadStoreIndexed},
            {"psq_lx"     , ParsePairedSingleLoadStoreIndexed},
            {"psq_st"     , ParsePairedSingleLoadStore       },
            {"psq_stu"    , ParsePairedSingleLoadStore       },
            {"psq_stux"   , ParsePairedSingleLoadStoreIndexed},
            {"psq_stx"    , ParsePairedSingleLoadStoreIndexed},
            {"ps_abs"     , ParsePairedSingleOneOperand      },
            {"ps_abs."    , ParsePairedSingleOneOperand      },
            {"ps_add"     , ParsePairedSingleTwoOperand      },
            {"ps_add."    , ParsePairedSingleTwoOperand      },
            {"ps_cmpo0"   , ParsePairedSingleCompare         },
            {"ps_cmpo1"   , ParsePairedSingleCompare         },
            {"ps_cmpu0"   , ParsePairedSingleCompare         },
            {"ps_cmpu1"   , ParsePairedSingleCompare         },
            {"ps_div"     , ParsePairedSingleTwoOperand      },
            {"ps_div."    , ParsePairedSingleTwoOperand      },
            {"ps_madd"    , ParsePairedSingleThreeOperand    },
            {"ps_madd."   , ParsePairedSingleThreeOperand    },
            {"ps_madds0"  , ParsePairedSingleThreeOperand    },
            {"ps_madds0." , ParsePairedSingleThreeOperand    },
            {"ps_madds1"  , ParsePairedSingleThreeOperand    },
            {"ps_madds1." , ParsePairedSingleThreeOperand    },
            {"ps_merge00" , ParsePairedSingleTwoOperand      },
            {"ps_merge00.", ParsePairedSingleTwoOperand      },
            {"ps_merge01" , ParsePairedSingleTwoOperand      },
            {"ps_merge01.", ParsePairedSingleTwoOperand      },
            {"ps_merge10" , ParsePairedSingleTwoOperand      },
            {"ps_merge10.", ParsePairedSingleTwoOperand      },
            {"ps_merge11" , ParsePairedSingleTwoOperand      },
            {"ps_merge11.", ParsePairedSingleTwoOperand      },
            {"ps_mr"      , ParsePairedSingleOneOperand      },
            {"ps_mr."     , ParsePairedSingleOneOperand      },
            {"ps_msub"    , ParsePairedSingleThreeOperand    },
            {"ps_msub."   , ParsePairedSingleThreeOperand    },
            {"ps_mul"     , ParsePairedSingleMultiply        },
            {"ps_mul."    , ParsePairedSingleMultiply        },
            {"ps_muls0"   , ParsePairedSingleMultiply        },
            {"ps_muls0."  , ParsePairedSingleMultiply        },
            {"ps_muls1"   , ParsePairedSingleMultiply        },
            {"ps_muls1."  , ParsePairedSingleMultiply        },
            {"ps_nabs"    , ParsePairedSingleOneOperand      },
            {"ps_nabs."   , ParsePairedSingleOneOperand      },
            {"ps_neg"     , ParsePairedSingleOneOperand      },
            {"ps_neg."    , ParsePairedSingleOneOperand      },
            {"ps_nmadd"   , ParsePairedSingleThreeOperand    },
            {"ps_nmadd."  , ParsePairedSingleThreeOperand    },
            {"ps_nmsub"   , ParsePairedSingleThreeOperand    },
            {"ps_nmsub."  , ParsePairedSingleThreeOperand    },
            {"ps_res"     , ParsePairedSingleOneOperand      },
            {"ps_res."    , ParsePairedSingleOneOperand      },
            {"ps_rsqrte"  , ParsePairedSingleOneOperand      },
            {"ps_rsqrte." , ParsePairedSingleOneOperand      },
            {"ps_sel"     , ParsePairedSingleThreeOperand    },
            {"ps_sel."    , ParsePairedSingleThreeOperand    },
            {"ps_sub"     , ParsePairedSingleTwoOperand      },
            {"ps_sub."    , ParsePairedSingleTwoOperand      },
            {"ps_sum0"    , ParsePairedSingleThreeOperand    },
            {"ps_sum0."   , ParsePairedSingleThreeOperand    },
            {"ps_sum1"    , ParsePairedSingleThreeOperand    },
            {"ps_sum1."   , ParsePairedSingleThreeOperand    },
            {"stb"        , ParseInstructionStoreInteger     },
            {"stbu"       , ParseInstructionStoreInteger     },
            {"stbux"      , ParseInstructionStoreInteger     },
            {"stbx"       , ParseInstructionStoreInteger     },
            {"stfd"       , ParseFloatingPointStore          },
            {"stfdu"      , ParseFloatingPointStore          },
            {"stfdux"     , ParseFloatingPointStoreIndexed   },
            {"stfdx"      , ParseFloatingPointStoreIndexed   },
            {"stfiwx"     , ParseFloatingPointStoreIndexed   },
            {"stfs"       , ParseFloatingPointStore          },
            {"stfsu"      , ParseFloatingPointStore          },
            {"stfsux"     , ParseFloatingPointStoreIndexed   },
            {"stfsx"      , ParseFloatingPointStoreIndexed   },
            {"sth"        , ParseInstructionStoreInteger     },
            {"sthbrx"     , ParseInstructionStoreInteger     },
            {"sthu"       , ParseInstructionStoreInteger     },
            {"sthux"      , ParseInstructionStoreInteger     },
            {"sthx"       , ParseInstructionStoreInteger     },
            {"stw"        , ParseInstructionStoreInteger     },
            {"stwbrx"     , ParseInstructionStoreInteger     },
            {"stwcx."     , ParseInstructionStoreInteger     },
            {"stwu"       , ParseInstructionStoreInteger     },
            {"stwux"      , ParseInstructionStoreInteger     },
            {"stwx"       , ParseInstructionStoreInteger     },
            {"sub"        , ParseInstructionSUBF             }, // Simplified mnemonic for subf
            {"subic"      , ParseInstructionADDIC            }, // Simplified mnemonic for addic
            {"subic."     , ParseInstructionADDIC            }, // Simplified mnemonic for addic.
            {"subf"       , ParseInstructionSUBF             },
            {"subf."      , ParseInstructionSUBF             },
            {"subfo"      , ParseInstructionSUBF             },
            {"subfo."     , ParseInstructionSUBF             },
            {"xori"       , ParseIntegerLogicalImmediate     },
            {"xoris"      , ParseIntegerLogicalImmediate     },
        };

        public List<IOptimizer> Optimizers { get; }

        public Assembler(IEnumerable<IOptimizer> optimizers)
        {
            Optimizers = new List<IOptimizer>(optimizers);
        }

        public Assembler()
        {
            Optimizers = new List<IOptimizer>() { new GeneralOptimizer() };
        }

        private static string reduceLineToCode(string line)
        {
            if (line.Contains(";"))
                line = line.Substring(0, line.IndexOf(";"));

            return line.Trim();
        }

        public IRCodeBlock AssembleAllLines(IEnumerable<string> lines)
        {
            var instructionPointer = 0x00000000;
            return assembleAllLinesRef(new Queue<string>(lines), ref instructionPointer);
        }

        private IEnumerable<string> GetCodeLines(Queue<string> lines)
        {
            while (lines.Any())
            {
                var line = lines.Dequeue();
                line = reduceLineToCode(line);
                if (!string.IsNullOrWhiteSpace(line))
                    yield return line;
            }
        }

        public IRCodeBlock assembleAllLines(Queue<string> lines, int instructionPointer)
        {
            return assembleAllLinesRef(lines, ref instructionPointer);
        }

        private IRCodeBlock assembleAllLinesRef(Queue<string> lines, ref int instructionPointer)
        {
            var units = new List<IIRUnit>();

            foreach (var line in GetCodeLines(lines))
            {
                if (line.EndsWith(":"))
                {
                    instructionPointer = ParseInstructionPointerLabel(line);
                }
                else if (line.StartsWith("!"))
                {
                    if (line == "!end")
                        break;

                    var specialInstruction = ParseSpecialInstruction(line.Substring(1), ref instructionPointer, lines);
                    units.Add(specialInstruction);
                }
                else if (line.StartsWith("."))
                {
                    var dataSection = ParseDataSection(line.Substring(1), instructionPointer);
                    units.Add(dataSection);
                    instructionPointer += dataSection.Length;
                }
                else
                {
                    //Align the instruction
                    instructionPointer = (instructionPointer + 3) & ~3;
                    var instruction = ParseInstruction(line, instructionPointer);
                    units.Add(instruction);
                    instructionPointer += 4;
                }
            }

            var block = units.ToCodeBlock();

            foreach (var optimizer in Optimizers)
            {
                block = optimizer.Optimize(block);
            }

            return block;
        }

        private IIRUnit ParseSpecialInstruction(string line, ref int instructionPointer, Queue<string> lines)
        {
            var tokens = TokenizeLine(line);

            switch (tokens[0])
            {
                case "u8mask":
                    return ParseUnsigned8Mask(tokens, instructionPointer, lines);
                case "u16mask":
                    return ParseUnsigned16Mask(tokens, instructionPointer, lines);

                case "u8equal":
                    return ParseUnsigned8Equal(tokens, instructionPointer, lines);
                case "u16equal":
                    return ParseUnsigned16Equal(tokens, instructionPointer, lines);
                case "u32equal":
                    return ParseUnsigned32Equal(tokens, instructionPointer, lines);
                case "s8equal":
                    return ParseSigned8Equal(tokens, instructionPointer, lines);
                case "s16equal":
                    return ParseSigned16Equal(tokens, instructionPointer, lines);
                case "s32equal":
                    return ParseSigned32Equal(tokens, instructionPointer, lines);
                case "f32equal":
                    return ParseFloat32Equal(tokens, instructionPointer, lines);

                case "u8unequal":
                    return ParseUnsigned8Unequal(tokens, instructionPointer, lines);
                case "u16unequal":
                    return ParseUnsigned16Unequal(tokens, instructionPointer, lines);
                case "u32unequal":
                    return ParseUnsigned32Unequal(tokens, instructionPointer, lines);
                case "s8unequal":
                    return ParseSigned8Unequal(tokens, instructionPointer, lines);
                case "s16unequal":
                    return ParseSigned16Unequal(tokens, instructionPointer, lines);
                case "s32unequal":
                    return ParseSigned32Unequal(tokens, instructionPointer, lines);
                case "f32unequal":
                    return ParseFloat32Unequal(tokens, instructionPointer, lines);

                case "u8lessthan":
                    return ParseUnsigned8LessThan(tokens, instructionPointer, lines);
                case "u16lessthan":
                    return ParseUnsigned16LessThan(tokens, instructionPointer, lines);
                case "u32lessthan":
                    return ParseUnsigned32LessThan(tokens, instructionPointer, lines);
                case "s8lessthan":
                    return ParseSigned8LessThan(tokens, instructionPointer, lines);
                case "s16lessthan":
                    return ParseSigned16LessThan(tokens, instructionPointer, lines);
                case "s32lessthan":
                    return ParseSigned32LessThan(tokens, instructionPointer, lines);
                case "f32lessthan":
                    return ParseFloat32LessThan(tokens, instructionPointer, lines);

                case "u8greaterthan":
                    return ParseUnsigned8GreaterThan(tokens, instructionPointer, lines);
                case "u16greaterthan":
                    return ParseUnsigned16GreaterThan(tokens, instructionPointer, lines);
                case "u32greaterthan":
                    return ParseUnsigned32GreaterThan(tokens, instructionPointer, lines);
                case "s8greaterthan":
                    return ParseSigned8GreaterThan(tokens, instructionPointer, lines);
                case "s16greaterthan":
                    return ParseSigned16GreaterThan(tokens, instructionPointer, lines);
                case "s32greaterthan":
                    return ParseSigned32GreaterThan(tokens, instructionPointer, lines);
                case "f32greaterthan":
                    return ParseFloat32GreaterThan(tokens, instructionPointer, lines);

                case "u8add":
                    return ParseUnsigned8Add(tokens, ref instructionPointer);
                case "u16add":
                    return ParseUnsigned16Add(tokens, ref instructionPointer);
                case "u32add":
                    return ParseUnsigned32Add(tokens, ref instructionPointer);
                case "s8add":
                    return ParseSigned8Add(tokens, ref instructionPointer);
                case "s16add":
                    return ParseSigned16Add(tokens, ref instructionPointer);
                case "s32add":
                    return ParseSigned32Add(tokens, ref instructionPointer);
                case "f32add":
                    return ParseFloat32Add(tokens, ref instructionPointer);

                case "u8bitset":
                    return ParseUnsigned8BitSet(tokens, ref instructionPointer);
                case "u16bitset":
                    return ParseUnsigned16BitSet(tokens, ref instructionPointer);
                case "u32bitset":
                    return ParseUnsigned32BitSet(tokens, ref instructionPointer);

                case "u8bitunset":
                    return ParseUnsigned8BitUnset(tokens, ref instructionPointer);
                case "u16bitunset":
                    return ParseUnsigned16BitUnset(tokens, ref instructionPointer);
                case "u32bitunset":
                    return ParseUnsigned32BitUnset(tokens, ref instructionPointer);

                case "repeat":
                    return ParseRepeat(tokens, ref instructionPointer, lines);
            }

            throw new ArgumentException($"The specified special instruction { tokens[0] } is not supported.");
        }

        private IIRUnit ParseRepeat(string[] tokens, ref int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var units = new List<IIRUnit>();
            for (var i = 0; i < value - 1; ++i)
            {
                units.AddRange(assembleAllLinesRef(new Queue<string>(lines), ref instructionPointer).Units);
            }
            var lastRepeat = assembleAllLinesRef(lines, ref instructionPointer).Units;
            if (value > 0)
            {
                units.AddRange(lastRepeat);
            }
            return new IRMultiUnit(units);
        }

        #region Mask

        private IIRUnit ParseUnsigned8Mask(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Mask(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Mask(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Mask(instructionPointer, value, block);
        }

        #endregion

        #region Setting Bits

        private IIRUnit ParseUnsigned8BitSet(string[] tokens, ref int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned8BitSet(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16BitSet(string[] tokens, ref int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned16BitSet(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32BitSet(string[] tokens, ref int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned32BitSet(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Unsetting Bits

        private IIRUnit ParseUnsigned8BitUnset(string[] tokens, ref int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned8BitUnset(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16BitUnset(string[] tokens, ref int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned16BitUnset(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32BitUnset(string[] tokens, ref int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned32BitUnset(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Add

        private IIRUnit ParseUnsigned8Add(string[] tokens, ref int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned8Add(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseUnsigned16Add(string[] tokens, ref int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned16Add(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseUnsigned32Add(string[] tokens, ref int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var result = new IRUnsigned32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        private IIRUnit ParseSigned8Add(string[] tokens, ref int instructionPointer)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var result = new IRSigned8Add(instructionPointer, value);
            ++instructionPointer;
            return result;
        }

        private IIRUnit ParseSigned16Add(string[] tokens, ref int instructionPointer)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var result = new IRSigned16Add(instructionPointer, value);
            instructionPointer += 2;
            return result;
        }

        private IIRUnit ParseSigned32Add(string[] tokens, ref int instructionPointer)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var result = new IRSigned32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        private IIRUnit ParseFloat32Add(string[] tokens, ref int instructionPointer)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var result = new IRFloat32Add(instructionPointer, value);
            instructionPointer += 4;
            return result;
        }

        #endregion

        #region Equal

        private IIRUnit ParseUnsigned8Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32Equal(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32Equal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32Equal(instructionPointer, value, block);
        }

        #endregion

        #region Unequal

        private IIRUnit ParseUnsigned8Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32Unequal(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32Unequal(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32Unequal(instructionPointer, value, block);
        }

        #endregion

        #region Less Than

        private IIRUnit ParseUnsigned8LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32LessThan(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32LessThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32LessThan(instructionPointer, value, block);
        }

        #endregion

        #region Greater Than

        private IIRUnit ParseUnsigned8GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned8GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned16GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned16GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseUnsigned32GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRUnsigned32GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned8GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned8GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned16GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned16GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseSigned32GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRSigned32GreaterThan(instructionPointer, value, block);
        }

        private IIRUnit ParseFloat32GreaterThan(string[] tokens, int instructionPointer, Queue<string> lines)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            var block = assembleAllLines(lines, instructionPointer);
            return new IRFloat32GreaterThan(instructionPointer, value, block);
        }

        #endregion

        private IRWriteData ParseDataSection(string line, int instructionPointer)
        {
            var tokens = TokenizeLine(line);

            switch (tokens[0])
            {
                case "str":
                    return ParseStringDataSection(line, instructionPointer);
                case "u8":
                    return ParseUnsigned8DataSection(tokens, instructionPointer);
                case "u16":
                    return ParseUnsigned16DataSection(tokens, instructionPointer);
                case "u32":
                    return ParseUnsigned32DataSection(tokens, instructionPointer);
                case "u64":
                    return ParseUnsigned64DataSection(tokens, instructionPointer);
                case "s8":
                    return ParseSigned8DataSection(tokens, instructionPointer);
                case "s16":
                    return ParseSigned16DataSection(tokens, instructionPointer);
                case "s32":
                    return ParseSigned32DataSection(tokens, instructionPointer);
                case "s64":
                    return ParseSigned64DataSection(tokens, instructionPointer);
                case "f32":
                    return ParseFloat32DataSection(tokens, instructionPointer);
                case "f64":
                    return ParseFloat64DataSection(tokens, instructionPointer);
            }

            throw new ArgumentException($"The specified data section { tokens[0] } is not supported.");
        }

        #region Data Sections

        private IRWriteData ParseStringDataSection(string line, int instructionPointer)
        {
            var literal = line.Substring(line.IndexOf("\""));
            var text = ParseStringLiteral(literal);
            return new StringDataSection(instructionPointer, text);
        }

        private string ParseStringLiteral(string literal)
        {
            return literal.Substring(1, literal.Length - 2);
        }

        private IRWriteData ParseUnsigned8DataSection(string[] tokens, int instructionPointer)
        {
            var value = (byte)ParseIntegerLiteral(tokens[1]);
            return new Unsigned8DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned16DataSection(string[] tokens, int instructionPointer)
        {
            var value = (ushort)ParseIntegerLiteral(tokens[1]);
            return new Unsigned16DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned32DataSection(string[] tokens, int instructionPointer)
        {
            var value = (uint)ParseIntegerLiteral(tokens[1]);
            return new Unsigned32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseUnsigned64DataSection(string[] tokens, int instructionPointer)
        {
            var value = (ulong)ParseInteger64Literal(tokens[1]);
            return new Unsigned64DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned8DataSection(string[] tokens, int instructionPointer)
        {
            var value = (sbyte)ParseIntegerLiteral(tokens[1]);
            return new Signed8DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned16DataSection(string[] tokens, int instructionPointer)
        {
            var value = (short)ParseIntegerLiteral(tokens[1]);
            return new Signed16DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned32DataSection(string[] tokens, int instructionPointer)
        {
            var value = ParseIntegerLiteral(tokens[1]);
            return new Signed32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseSigned64DataSection(string[] tokens, int instructionPointer)
        {
            var value = ParseInteger64Literal(tokens[1]);
            return new Signed64DataSection(instructionPointer, value);
        }

        private IRWriteData ParseFloat32DataSection(string[] tokens, int instructionPointer)
        {
            var value = (float)ParseFloatLiteral(tokens[1]);
            return new Float32DataSection(instructionPointer, value);
        }

        private IRWriteData ParseFloat64DataSection(string[] tokens, int instructionPointer)
        {
            var value = ParseFloatLiteral(tokens[1]);
            return new Float64DataSection(instructionPointer, value);
        }

        #endregion

        private static GekkoInstruction ParseInstruction(string line, int instructionPointer)
        {
            var tokens = TokenizeLine(line);

            // Lowercase the instruction name.
            // Handles stylistic forms where someone might
            // prefer uppercasing their assembly instructions.
            tokens[0] = tokens[0].ToLower();

            if (instructionTable.ContainsKey(tokens[0]))
                return instructionTable[tokens[0]](tokens, instructionPointer);

            throw new ArgumentException($"The specified instruction { tokens[0] } is not supported.");
        }

        #region Gekko Instructions

        private static GekkoInstruction ParseInstructionADD(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new AddInstruction(instructionPointer, rd, ra, rb, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDC(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new AddCarryingInstruction(instructionPointer, rd, ra, rb, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDE(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new AddExtendedInstruction(instructionPointer, rd, ra, rb, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDI(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var simm = ParseIntegerLiteral(tokens[3]);
            return new AddImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private static GekkoInstruction ParseInstructionADDME(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            return new AddToMinusOneExtendedInstruction(instructionPointer, rd, ra, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDZE(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            return new AddToZeroExtendedInstruction(instructionPointer, rd, ra, tokens[0].Contains("o"), tokens[0].EndsWith("."));
        }

        private static GekkoInstruction ParseInstructionADDIC(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            int simm = ParseIntegerLiteral(tokens[3]);

            if (tokens[0] == "addic")
                return new AddImmediateCarryingInstruction(instructionPointer, rd, ra, simm);
            if (tokens[0] == "addic.")
                return new AddImmediateCarryingAndRecordInstruction(instructionPointer, rd, ra, simm);

            // subic mnemonics
            if (tokens[0] == "subic")
                return new AddImmediateCarryingInstruction(instructionPointer, rd, ra, -simm);
            // subic.
            return new AddImmediateCarryingAndRecordInstruction(instructionPointer, rd, ra, -simm);
        }

        private static GekkoInstruction ParseInstructionADDIS(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var simm = ParseIntegerLiteral(tokens[3]);
            return new AddImmediateShiftedInstruction(instructionPointer, rd, ra, simm);
        }

        private static GekkoInstruction ParseInstructionAND(string[] tokens, int instructionPointer)
        {
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);

            if (tokens[0].Contains("i"))
            {
                var uimm = ParseIntegerLiteral(tokens[3]);
                var shifted = tokens[0].Contains("s");

                return new AndImmediateInstruction(instructionPointer, ra, rs, uimm, shifted);
            }

            var rb = ParseRegister(tokens[3]);
            var rc = tokens[0].EndsWith(".");
            var complement = tokens[0].Contains("c");

            return new AndInstruction(instructionPointer, ra, rs, rb, rc, complement);
        }

        private static GekkoInstruction ParseInstructionB(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, false, false);
        }

        private static GekkoInstruction ParseInstructionBA(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, true, false);
        }

        private static GekkoInstruction ParseInstructionBL(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, false, true);
        }

        private static GekkoInstruction ParseInstructionBLA(string[] tokens, int instructionPointer)
        {
            var targetAddress = ParseIntegerLiteral(tokens[1]);
            return new BranchInstruction(instructionPointer, targetAddress, true, true);
        }

        private static GekkoInstruction ParseInstructionBLR(string[] tokens, int instructionPointer)
        {
            return new BranchToLinkRegisterInstruction(instructionPointer);
        }

        private static GekkoInstruction ParseInstructionCMP(string[] tokens, int instructionPointer)
        {
            bool immediate       = tokens[0].Contains("i");
            bool logical_variant = tokens[0].Contains("l");
            bool w_mnemonic      = tokens[0].Contains("w");

            var crfd = ParseConditionRegister(tokens[1]);

            if (w_mnemonic)
            {
                var ra = ParseRegister(tokens[2]);

                if (immediate)
                {
                    var imm = ParseIntegerLiteral(tokens[3]);
                    return new CompareImmediateInstruction(instructionPointer, crfd, 0, ra, imm, logical_variant);
                }

                var rb = ParseRegister(tokens[3]);
                return new CompareInstruction(instructionPointer, crfd, 0, ra, rb, logical_variant);
            }
            else
            {
                var ra = ParseRegister(tokens[3]);
                var L = ParseIntegerLiteral(tokens[2]);

                if (immediate)
                {
                    var imm = ParseIntegerLiteral(tokens[4]);
                    return new CompareImmediateInstruction(instructionPointer, crfd, L, ra, imm, logical_variant);
                }

                var rb = ParseRegister(tokens[4]);
                return new CompareInstruction(instructionPointer, crfd, L, ra, rb, logical_variant);
            }
        }

        private static GekkoInstruction ParseInstructionCNTLZW(string[] tokens, int instructionPointer)
        {
            var rc = tokens[0].EndsWith(".");
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);

            return new CountLeadingZeroesWordInstruction(instructionPointer, ra, rs, rc);
        }

        private static GekkoInstruction ParseInstructionCRAND(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterANDInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRANDC(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterANDComplementInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRCLR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            return new ConditionRegisterClearInstruction(instructionPointer, crbd);
        }

        private static GekkoInstruction ParseInstructionCREQV(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterEquivalentInstruction(instructionPointer, crbd, crba, crbb);
        }

        // Simplified mnemonic of CROR
        private static GekkoInstruction ParseInstructionCRMOVE(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            return new ConditionRegisterORInstruction(instructionPointer, crbd, crba, crba);
        }

        private static GekkoInstruction ParseInstructionCRNAND(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterNANDInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRNOR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterNORInstruction(instructionPointer, crbd, crba, crbb);
        }

        // Simplified mnemonic of CRNOR
        private static GekkoInstruction ParseInstructionCRNOT(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            return new ConditionRegisterNORInstruction(instructionPointer, crbd, crba, crba);
        }

        private static GekkoInstruction ParseInstructionCROR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRORC(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterORComplementInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionCRSET(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            return new ConditionRegisterSetInstruction(instructionPointer, crbd);
        }

        private static GekkoInstruction ParseInstructionCRXOR(string[] tokens, int instructionPointer)
        {
            var crbd = ParseConditionRegister(tokens[1]);
            var crba = ParseConditionRegister(tokens[2]);
            var crbb = ParseConditionRegister(tokens[3]);
            return new ConditionRegisterXORInstruction(instructionPointer, crbd, crba, crbb);
        }

        private static GekkoInstruction ParseInstructionDataCache(string[] tokens, int instructionPointer)
        {
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);

            switch (tokens[0])
            {
                case "dcbf":
                    return new DataCacheInstruction(instructionPointer, DataCacheInstruction.PrimaryOpcode.NonDCBZ_L, DataCacheInstruction.SecondaryOpcode.DCBF, ra, rs);
                case "dcbi":
                    return new DataCacheInstruction(instructionPointer, DataCacheInstruction.PrimaryOpcode.NonDCBZ_L, DataCacheInstruction.SecondaryOpcode.DCBI, ra, rs);
                case "dcbst":
                    return new DataCacheInstruction(instructionPointer, DataCacheInstruction.PrimaryOpcode.NonDCBZ_L, DataCacheInstruction.SecondaryOpcode.DCBST, ra, rs);
                case "dcbt":
                    return new DataCacheInstruction(instructionPointer, DataCacheInstruction.PrimaryOpcode.NonDCBZ_L, DataCacheInstruction.SecondaryOpcode.DCBT, ra, rs);
                case "dcbtst":
                    return new DataCacheInstruction(instructionPointer, DataCacheInstruction.PrimaryOpcode.NonDCBZ_L, DataCacheInstruction.SecondaryOpcode.DCBTST, ra, rs);
                case "dcbz":
                    return new DataCacheInstruction(instructionPointer, DataCacheInstruction.PrimaryOpcode.NonDCBZ_L, DataCacheInstruction.SecondaryOpcode.DCBZ, ra, rs);
            }

            return new DataCacheInstruction(instructionPointer, DataCacheInstruction.PrimaryOpcode.DCBZ_L, DataCacheInstruction.SecondaryOpcode.DCBZ_L, ra, rs);
        }

        private static GekkoInstruction ParseInstructionDIVW(string[] tokens, int instructionPointer)
        {
            var rc = tokens[0].EndsWith(".");
            var oe = tokens[0].Contains("o");
            var unsigned = tokens[0].Contains("u");

            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);

            return new DivideWordInstruction(instructionPointer, rd, ra, rb, oe, rc, unsigned);
        }

        private static GekkoInstruction ParseInstructionEIEIO(string[] tokens, int instructionPointer)
        {
            return new EnforceInOrderExecutionInstruction(instructionPointer);
        }

        private static GekkoInstruction ParseInstructionEQV(string[] tokens, int instructionPointer)
        {
            var rc = tokens[0].EndsWith(".");
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);

            return new EquivalentInstruction (instructionPointer, ra, rs, rb, rc);
        }

        private static GekkoInstruction ParseInstructionExternalControl(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);

            var opcode = (tokens[0] == "eciwx") ? ExternalControlInstruction.Opcode.ECIWX
                                                : ExternalControlInstruction.Opcode.ECOWX;

            return new ExternalControlInstruction(instructionPointer, rd, ra, rb, opcode);
        }

        private static GekkoInstruction ParseFloatingPointCompare(string[] tokens, int instructionPointer)
        {
            var crfd = ParseConditionRegister(tokens[1]);
            var fra = ParseRegister(tokens[2]);
            var frb = ParseRegister(tokens[3]);
            var opcode = FloatingPointCompareInstruction.Opcode.FCMPO;

            if (tokens[0].Contains("u"))
                opcode = FloatingPointCompareInstruction.Opcode.FCMPU;

            return new FloatingPointCompareInstruction(instructionPointer, crfd, fra, frb, opcode);
        }

        private static GekkoInstruction ParseFloatingPointMultiply(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var frd = ParseRegister(tokens[1]);
            var fra = ParseRegister(tokens[2]);
            var frc = ParseRegister(tokens[3]);
            var rc  = opname.EndsWith(".");

            var singleIndex = opname.LastIndexOf('s');
            var single = singleIndex == opname.Length - 1 || singleIndex == opname.Length - 2;

            var variant = single ? FloatingPointMultiplyInstruction.Variant.Single
                                 : FloatingPointMultiplyInstruction.Variant.Double;

            return new FloatingPointMultiplyInstruction(instructionPointer, frd, fra, frc, rc, variant);
        }

        private static GekkoInstruction ParseFloatingPointSingleOperand(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var frd = ParseRegister(tokens[1]);
            var frb = ParseRegister(tokens[2]);
            var rc  = opname.EndsWith(".");

            var opcode = FloatingPointSingleOperandInstruction.Opcode.FABS;

            if (opname.StartsWith("fctiwz"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FCTIWZ;
            else if (opname.StartsWith("fctiw"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FCTIW;
            else if (opname.StartsWith("fmr"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FMR;
            else if (opname.StartsWith("fnabs"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FNABS;
            else if (opname.StartsWith("fneg"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FNEG;
            else if (opname.StartsWith("fres"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FRES;
            else if (opname.StartsWith("frsp"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FRSP;
            else if (opname.StartsWith("frsqrte"))
                opcode = FloatingPointSingleOperandInstruction.Opcode.FRSQRTE;

            return new FloatingPointSingleOperandInstruction(instructionPointer, frd, frb, rc, opcode);
        }

        private static GekkoInstruction ParseFloatingPointThreeOperand(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var frd = ParseRegister(tokens[1]);
            var fra = ParseRegister(tokens[2]);
            var frc = ParseRegister(tokens[3]);
            var frb = ParseRegister(tokens[4]);
            var rc = opname.EndsWith(".");

            var singleIndex = opname.LastIndexOf('s');
            var single = singleIndex == opname.Length - 1 || singleIndex == opname.Length - 2;

            var variant = single ? FloatingPointThreeOperandInstruction.Variant.Single
                                 : FloatingPointThreeOperandInstruction.Variant.Double;

            var opcode = FloatingPointThreeOperandInstruction.Opcode.FSEL;

            if (opname.StartsWith("fmadd"))
                opcode = FloatingPointThreeOperandInstruction.Opcode.FMADD;
            else if (opname.StartsWith("fmsub"))
                opcode = FloatingPointThreeOperandInstruction.Opcode.FMSUB;
            else if (opname.StartsWith("fnmadd"))
                opcode = FloatingPointThreeOperandInstruction.Opcode.FNMADD;
            else if (opname.StartsWith("fnmsub"))
                opcode = FloatingPointThreeOperandInstruction.Opcode.FNMSUB;

            return new FloatingPointThreeOperandInstruction(instructionPointer, frd, fra, frc, frb, rc, variant, opcode);
        }

        private static GekkoInstruction ParseFloatingPointTwoOperand(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var frd = ParseRegister(tokens[1]);
            var fra = ParseRegister(tokens[2]);
            var frb = ParseRegister(tokens[3]);
            var rc = opname.EndsWith(".");

            var singleIndex = opname.LastIndexOf('s');
            var single = singleIndex == opname.Length - 1 || singleIndex == opname.Length - 2;

            var variant = single ? FloatingPointTwoOperandInstruction.Variant.Single
                                 : FloatingPointTwoOperandInstruction.Variant.Double;

            var opcode = FloatingPointTwoOperandInstruction.Opcode.FADD;

            if (opname.StartsWith("fdiv"))
                opcode = FloatingPointTwoOperandInstruction.Opcode.FDIV;
            else if (opname.StartsWith("fsub"))
                opcode = FloatingPointTwoOperandInstruction.Opcode.FSUB;

            return new FloatingPointTwoOperandInstruction(instructionPointer, frd, fra, frb, rc, variant, opcode);
        }

        private static GekkoInstruction ParseIntegerLogicalImmediate(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);
            var imm = ParseIntegerLiteral(tokens[3]);

            var opcode = IntegerLogicalImmediateInstruction.Opcode.ANDI;

            switch (opname)
            {
                case "andis.":
                    opcode = IntegerLogicalImmediateInstruction.Opcode.ANDIS;
                    break;
                case "ori":
                    opcode = IntegerLogicalImmediateInstruction.Opcode.ORI;
                    break;
                case "oris":
                    opcode = IntegerLogicalImmediateInstruction.Opcode.ORIS;
                    break;
                case "xori":
                    opcode = IntegerLogicalImmediateInstruction.Opcode.XORI;
                    break;
                case "xoris":
                    opcode = IntegerLogicalImmediateInstruction.Opcode.XORIS;
                    break;
            }

            return new IntegerLogicalImmediateInstruction(instructionPointer, ra, rs, imm, opcode);
        }

        private static GekkoInstruction ParseInstructionICBI(string[] tokens, int instructionPointer)
        {
            var ra = ParseRegister(tokens[1]);
            var rb = ParseRegister(tokens[2]);
            return new InstructionCacheBlockInvalidateInstruction(instructionPointer, ra, rb);
        }

        private static GekkoInstruction ParseInstructionISYNC(string[] tokens, int instructionPointer)
        {
            return new InstructionSynchronizeInstruction(instructionPointer);
        }

        private static GekkoInstruction ParseLoadFloatingPoint(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var single = opname.Contains("s");
            var update = opname.Contains("u");

            var frd    = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra     = ParseRegister(tokens[3]);

            if (update && ra == 0)
                throw new ArgumentException($"{opname} cannot have register rA specified as 0.");

            var opcode = LoadFloatingPointInstruction.Opcode.LFS;

            if (single && update)
            {
                opcode = LoadFloatingPointInstruction.Opcode.LFSU;
            }
            else if (!single)
            {
                if (update)
                    opcode = LoadFloatingPointInstruction.Opcode.LFDU;
                else
                    opcode = LoadFloatingPointInstruction.Opcode.LFD;
            }

            return new LoadFloatingPointInstruction(instructionPointer, frd, offset, ra, opcode);
        }

        private static GekkoInstruction ParseLoadFloatingPointIndexed(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var single = opname.Contains("s");
            var update = opname.Contains("u");

            var frd = ParseRegister(tokens[1]);
            var ra  = ParseRegister(tokens[2]);
            var rb  = ParseRegister(tokens[3]);

            if (update && ra == 0)
                throw new ArgumentException($"{opname} cannot have register rA specified as 0.");

            var opcode = LoadFloatingPointIndexedInstruction.Opcode.LFSX;

            if (single && update)
            {
                opcode = LoadFloatingPointIndexedInstruction.Opcode.LFSUX;
            }
            else if (!single)
            {
                if (update)
                    opcode = LoadFloatingPointIndexedInstruction.Opcode.LFDUX;
                else
                    opcode = LoadFloatingPointIndexedInstruction.Opcode.LFDX;
            }

            return new LoadFloatingPointIndexedInstruction(instructionPointer, frd, ra, rb, opcode);
        }

        private static GekkoInstruction ParseInstructionLoadInteger(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
           
            var algebraic = tokens[0].Contains("a");
            var indexed   = tokens[0].EndsWith("x");
            var updating  = tokens[0].Contains("u");
            var zero      = tokens[0].Contains("z");

            // Ensures that rA isn't zero or is the same as the destination
            // register for update instruction variants.
            Action<int, int> validateUpdatingVariantOperands = (src, dest) => {
                if (src == 0)
                    throw new FormatException($"{tokens[0]} cannot have a rA as zero");
                if (src == dest)
                    throw new FormatException($"{tokens[0]} cannot have rA and rD as the same register");
            };

            if (indexed)
            {
                var ra = ParseRegister(tokens[2]);
                var rb = ParseRegister(tokens[3]);

                if (updating)
                    validateUpdatingVariantOperands(ra, rd);

                // Halfword
                if (tokens[0].Contains("h"))
                {
                    var opcode = LoadHalfWordIndexedInstruction.Opcode.LHBRX;

                    if (algebraic)
                    {
                        if (updating)
                            opcode = LoadHalfWordIndexedInstruction.Opcode.LHAUX;
                        else
                            opcode = LoadHalfWordIndexedInstruction.Opcode.LHAX;
                    }
                    else if (zero)
                    {
                        if (updating)
                            opcode = LoadHalfWordIndexedInstruction.Opcode.LHZUX;
                        else
                            opcode = LoadHalfWordIndexedInstruction.Opcode.LHZX;
                    }

                    return new LoadHalfWordIndexedInstruction(instructionPointer, rd, ra, rb, opcode);
                }
                // Word
                else if (tokens[0].Contains("w"))
                {
                    var opcode = algebraic ? LoadWordIndexedInstruction.Opcode.LWARX :
                                 updating  ? LoadWordIndexedInstruction.Opcode.LWZUX :
                                 zero      ? LoadWordIndexedInstruction.Opcode.LWZX
                                           : LoadWordIndexedInstruction.Opcode.LWBRX;

                    return new LoadWordIndexedInstruction(instructionPointer, rd, ra, rb, opcode);
                }
                else // Byte
                {
                    var opcode = updating ? LoadByteIndexedInstruction.Opcode.LBZUX
                                          : LoadByteIndexedInstruction.Opcode.LBZX;

                    return new LoadByteIndexedInstruction(instructionPointer, rd, ra, rb, opcode);
                }
            }
            else
            {
                var offset = ParseIntegerLiteral(tokens[2]);
                var ra     = ParseRegister(tokens[3]);

                if (updating)
                    validateUpdatingVariantOperands(ra, rd);

                // Halfword
                if (tokens[0].Contains("h"))
                {
                    var opcode = LoadHalfWordInstruction.Opcode.LHA;

                    if (algebraic)
                    {
                        if (updating)
                            opcode = LoadHalfWordInstruction.Opcode.LHAU;
                    }
                    else if (zero)
                    {
                        if (updating)
                            opcode = LoadHalfWordInstruction.Opcode.LHZU;
                        else
                            opcode = LoadHalfWordInstruction.Opcode.LHZ;
                    }

                    return new LoadHalfWordInstruction(instructionPointer, rd, offset, ra, opcode);
                }
                // Word
                else if (tokens[0].Contains("w"))
                {
                    var opcode = updating ? LoadWordInstruction.Opcode.LWZU
                                          : LoadWordInstruction.Opcode.LWZ;

                    return new LoadWordInstruction(instructionPointer, rd, offset, ra, opcode);
                }
                else // Byte
                {
                    var opcode = updating ? LoadByteInstruction.Opcode.LBZU
                                          : LoadByteInstruction.Opcode.LBZ;

                    return new LoadByteInstruction(instructionPointer, rd, offset, ra, opcode);
                }
            }
        }

        private static GekkoInstruction ParseInstructionLIS(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var simm = ParseIntegerLiteral(tokens[2]);
            return new LoadImmediateShiftedInstruction(instructionPointer, rd, simm);
        }

        private static GekkoInstruction ParseInstructionMoveToCR(string[] tokens, int instructionPointer)
        {
            var crfd = ParseConditionRegister(tokens[1]);

            if (tokens[0] == "mcrxr")
                return new MoveToConditionRegisterInstruction(instructionPointer, crfd);

            var crfs = ParseConditionRegister(tokens[2]);
            var fromFPSCR = tokens[0].EndsWith("s");

            return new MoveToConditionRegisterInstruction(instructionPointer, crfd, crfs, fromFPSCR);
        }

        private static GekkoInstruction ParseInstructionMFCR(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);

            return new MoveFromConditionRegisterInstruction(instructionPointer, rd);
        }

        private static GekkoInstruction ParseInstructionMFFS(string[] tokens, int instructionPointer)
        {
            var rc  = tokens[0].EndsWith(".");
            var frd = ParseRegister(tokens[1]);

            return new MoveFromFPSCRInstruction(instructionPointer, frd, rc);
        }

        private static GekkoInstruction ParseInstructionMFLR(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            return new MoveFromLinkRegisterInstruction(instructionPointer, rd);
        }


        private static GekkoInstruction ParseInstructionMFMSR(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);

            return new MoveFromMSRInstruction(instructionPointer, rd);
        }

        private static GekkoInstruction ParseInstructionMFSR(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var sr = ParseSegmentRegister(tokens[2]);

            return new MoveFromSegmentRegisterInstruction(instructionPointer, rd, sr);
        }

        private static GekkoInstruction ParseInstructionMFSRIN(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var sr = ParseRegister(tokens[2]);

            return new MoveFromSegmentRegisterIndirectInstruction(instructionPointer, rd, sr);
        }

        private static GekkoInstruction ParseInstructionMFSPR(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var spr = ParseSpecialPurposeRegister(tokens[2]);
            return new MoveFromSpecialPurposeRegisterInstruction(instructionPointer, rd, spr);
        }

        private static GekkoInstruction ParseInstructionMFTB(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);

            // Short-hand mnemonic for the upper time base bits
            if (tokens[0].EndsWith("u"))
                return new MoveFromTimeBaseInstruction(instructionPointer, rd, 269);

            // Short-hand mnemonic for the lower time base bits
            if (tokens.Length == 2)
                return new MoveFromTimeBaseInstruction(instructionPointer, rd, 268);

            var tb = ParseTimeBaseRegister(tokens[2]);
            return new MoveFromTimeBaseInstruction(instructionPointer, rd, tb);
        }

        private static GekkoInstruction ParseInstructionMTLR(string[] tokens, int instructionPointer)
        {
            var rs = ParseRegister(tokens[1]);
            return new MoveToLinkRegisterInstruction(instructionPointer, rs);
        }

        private static GekkoInstruction ParseInstructionMTSPR(string[] tokens, int instructionPointer)
        {
            var spr = ParseSpecialPurposeRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);
            return new MoveToSpecialPurposeRegisterInstruction(instructionPointer, spr, rs);
        }

        private static GekkoInstruction ParseInstructionMULLI(string[] tokens, int instructionPointer)
        {
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var simm = ParseIntegerLiteral(tokens[3]);
            return new MultiplyLowImmediateInstruction(instructionPointer, rd, ra, simm);
        }

        private static GekkoInstruction ParseInstructionMULLW(string[] tokens, int instructionPointer)
        {
            var oe = tokens[0].Contains("o");
            var rc = tokens[0].EndsWith(".");
            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);
            return new MultiplyLowWordInstruction(instructionPointer, rd, ra, rb, oe, rc);
        }

        private static GekkoInstruction ParseInstructionNOP(string[] tokens, int instructionPointer)
        {
            return new IntegerLogicalImmediateInstruction(instructionPointer, 0, 0, 0, IntegerLogicalImmediateInstruction.Opcode.ORI);
        }

        private static GekkoInstruction ParseInstructionORI(string[] tokens, int instructionPointer)
        {
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);
            var uimm = ParseIntegerLiteral(tokens[3]);
            return new OrImmediateInstruction(instructionPointer, ra, rs, uimm);
        }

        private static GekkoInstruction ParsePairedSingleCompare(string[] tokens, int instructionPointer)
        {
            var unordered = tokens[0].Contains("u");
            var high      = tokens[0].Contains("0");

            var crfd   = ParseConditionRegister(tokens[1]);
            var fra    = ParseRegister(tokens[2]);
            var frb    = ParseRegister(tokens[3]);
            var opcode = PairedSingleCompareInstruction.Opcode.PS_CMPO1;

            if (unordered)
            {
                if (high)
                    opcode = PairedSingleCompareInstruction.Opcode.PS_CMPU0;
                else
                    opcode = PairedSingleCompareInstruction.Opcode.PS_CMPU1;
            }
            else if (high)
            {
                opcode = PairedSingleCompareInstruction.Opcode.PS_CMPO0;
            }

            return new PairedSingleCompareInstruction(instructionPointer, crfd, fra, frb, opcode);
        }

        private static GekkoInstruction ParsePairedSingleLoadStore(string[] tokens, int instructionPointer)
        {
            var load     = tokens[0].Contains("l");
            var updating = tokens[0].Contains("u");

            var frd    = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra     = ParseRegister(tokens[3]);
            var w      = ParseIntegerLiteral(tokens[4]) == 1;
            var i      = ParseIntegerLiteral(tokens[5]);

            var opcode = PairedSingleLoadStoreInstruction.Opcode.PSQ_L;

            if (updating && ra == 0)
                throw new ArgumentException($"{tokens[0]} cannot have register rA specified as 0.");

            if (load && updating)
            {
                opcode = PairedSingleLoadStoreInstruction.Opcode.PSQ_LU;
            }
            else if (!load)
            {
                if (updating)
                    opcode = PairedSingleLoadStoreInstruction.Opcode.PSQ_STU;
                else
                    opcode = PairedSingleLoadStoreInstruction.Opcode.PSQ_ST;
            }

            return new PairedSingleLoadStoreInstruction(instructionPointer, frd, offset, ra, w, i, opcode);
        }

        private static GekkoInstruction ParsePairedSingleLoadStoreIndexed(string[] tokens, int instructionPointer)
        {
            var load     = tokens[0].Contains("l");
            var updating = tokens[0].Contains("u");

            var frs = ParseRegister(tokens[1]);
            var ra  = ParseRegister(tokens[2]);
            var rb  = ParseRegister(tokens[3]);
            var w   = ParseIntegerLiteral(tokens[4]) == 1;
            var i   = ParseIntegerLiteral(tokens[5]);

            var opcode = PairedSingleLoadStoreIndexedInstruction.Opcode.PSQ_LX;

            if (updating && ra == 0)
                throw new ArgumentException($"{tokens[0]} cannot have register rA specified as 0.");

            if (load && updating)
            {
                opcode = PairedSingleLoadStoreIndexedInstruction.Opcode.PSQ_LUX;
            }
            else if (!load)
            {
                if (updating)
                    opcode = PairedSingleLoadStoreIndexedInstruction.Opcode.PSQ_STUX;
                else
                    opcode = PairedSingleLoadStoreIndexedInstruction.Opcode.PSQ_STX;
            }

            return new PairedSingleLoadStoreIndexedInstruction(instructionPointer, frs, ra, rb, w, i, opcode);
        }

        private static GekkoInstruction ParsePairedSingleMultiply(string[] tokens, int instructionPointer)
        {
            var rc = tokens[0].EndsWith(".");
            var frd = ParseRegister(tokens[1]);
            var fra = ParseRegister(tokens[2]);
            var frc = ParseRegister(tokens[3]);
            var opcode = PairedSingleMultiplyInstruction.Opcode.PS_MUL;

            if (tokens[0].Contains("0"))
                opcode = PairedSingleMultiplyInstruction.Opcode.PS_MULS0;
            else if (tokens[0].Contains("1"))
                opcode = PairedSingleMultiplyInstruction.Opcode.PS_MULS1;

            return new PairedSingleMultiplyInstruction(instructionPointer, frd, fra, frc, rc, opcode);
        }

        private static GekkoInstruction ParsePairedSingleTwoOperand(string[] tokens, int instructionPointer)
        {
            var rc  = tokens[0].EndsWith(".");
            var frd = ParseRegister(tokens[1]);
            var fra = ParseRegister(tokens[2]);
            var frb = ParseRegister(tokens[3]);
            var opcode = PairedSingleTwoOperandInstruction.Opcode.PS_ADD;

            if (tokens[0].Contains("ps_div"))
                opcode = PairedSingleTwoOperandInstruction.Opcode.PS_DIV;
            else if (tokens[0].Contains("ps_merge00"))
                opcode = PairedSingleTwoOperandInstruction.Opcode.PS_MERGE00;
            else if (tokens[0].Contains("ps_merge01"))
                opcode = PairedSingleTwoOperandInstruction.Opcode.PS_MERGE01;
            else if (tokens[0].Contains("ps_merge10"))
                opcode = PairedSingleTwoOperandInstruction.Opcode.PS_MERGE10;
            else if (tokens[0].Contains("ps_merge11"))
                opcode = PairedSingleTwoOperandInstruction.Opcode.PS_MERGE11;
            else if (tokens[0].Contains("ps_sub"))
                opcode = PairedSingleTwoOperandInstruction.Opcode.PS_SUB;

            return new PairedSingleTwoOperandInstruction(instructionPointer, frd, fra, frb, rc, opcode);
        }

        private static GekkoInstruction ParsePairedSingleOneOperand(string[] tokens, int instructionPointer)
        {
            var rc  = tokens[0].EndsWith(".");
            var frd = ParseRegister(tokens[1]);
            var frb = ParseRegister(tokens[2]);
            var opcode = PairedSingleOneOperandInstruction.Opcode.PS_ABS;

            if (tokens[0].Contains("ps_mr"))
                opcode = PairedSingleOneOperandInstruction.Opcode.PS_MR;
            else if (tokens[0].Contains("ps_nabs"))
                opcode = PairedSingleOneOperandInstruction.Opcode.PS_NABS;
            else if (tokens[0].Contains("ps_neg"))
                opcode = PairedSingleOneOperandInstruction.Opcode.PS_NEG;
            else if (tokens[0].Contains("ps_res"))
                opcode = PairedSingleOneOperandInstruction.Opcode.PS_RES;
            else if (tokens[0].Contains("ps_rsqrte"))
                opcode = PairedSingleOneOperandInstruction.Opcode.PS_RSQRTE;

            return new PairedSingleOneOperandInstruction(instructionPointer, frd, frb, rc, opcode);
        }

        private static GekkoInstruction ParsePairedSingleThreeOperand(string[] tokens, int instructionPointer)
        {
            var rc = tokens[0].EndsWith(".");
            var frd = ParseRegister(tokens[1]);
            var fra = ParseRegister(tokens[2]);
            var frc = ParseRegister(tokens[3]);
            var frb = ParseRegister(tokens[4]);
            var opcode = PairedSingleThreeOperandInstruction.Opcode.PS_MADD;

            if (tokens[0].Contains("ps_madds0"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_MADDS0;
            else if (tokens[0].Contains("ps_madds1"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_MADDS1;
            else if (tokens[0].Contains("ps_msub"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_MSUB;
            else if (tokens[0].Contains("ps_nmadd"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_NMADD;
            else if (tokens[0].Contains("ps_nmsub"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_NMSUB;
            else if (tokens[0].Contains("ps_sel"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_SEL;
            else if (tokens[0].Contains("ps_sum0"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_SUM0;
            else if (tokens[0].Contains("ps_sum1"))
                opcode = PairedSingleThreeOperandInstruction.Opcode.PS_SUM1;

            return new PairedSingleThreeOperandInstruction(instructionPointer, frd, fra, frc, frb, rc, opcode);
        }

        private static GekkoInstruction ParseFloatingPointStore(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var frs    = ParseRegister(tokens[1]);
            var offset = ParseIntegerLiteral(tokens[2]);
            var ra     = ParseRegister(tokens[3]);

            var single = !opname.Contains("d");
            var update = opname.Contains("u");
            var opcode = StoreFloatingPointInstruction.Opcode.STFS;

            if (update && ra == 0)
                throw new ArgumentException($"{opname} cannot have register rA specified as 0.");

            if (single && update)
            {
                opcode = StoreFloatingPointInstruction.Opcode.STFSU;
            }
            else if (!single)
            {
                opcode = update ? StoreFloatingPointInstruction.Opcode.STFDU
                                : StoreFloatingPointInstruction.Opcode.STFD;
            }

            return new StoreFloatingPointInstruction(instructionPointer, frs, offset, ra, opcode);
        }

        private static GekkoInstruction ParseFloatingPointStoreIndexed(string[] tokens, int instructionPointer)
        {
            var opname = tokens[0];
            var frs    = ParseRegister(tokens[1]);
            var ra     = ParseRegister(tokens[2]);
            var rb     = ParseRegister(tokens[3]);

            var single = !opname.Contains("d");
            var update = opname.Contains("u");
            var word   = opname.Contains("w");
            var opcode = StoreFloatingPointIndexedInstruction.Opcode.STFIWX;

            if (update && ra == 0)
                throw new ArgumentException($"{opname} cannot have register rA specified as 0.");

            if (!word)
            {
                if (single)
                {
                    opcode = update ? StoreFloatingPointIndexedInstruction.Opcode.STFSUX
                                    : StoreFloatingPointIndexedInstruction.Opcode.STFSX;
                }
                else
                {
                    opcode = update ? StoreFloatingPointIndexedInstruction.Opcode.STFDUX
                                    : StoreFloatingPointIndexedInstruction.Opcode.STFDX;
                }
            }

            return new StoreFloatingPointIndexedInstruction(instructionPointer, frs, ra, rb, opcode);
        }

        private static GekkoInstruction ParseInstructionSignExtension(string[] tokens, int instructionPointer)
        {
            var rc = tokens[0].EndsWith(".");
            var ra = ParseRegister(tokens[1]);
            var rs = ParseRegister(tokens[2]);

            var opcode = tokens[0].Contains("h") ? SignExtensionInstruction.Opcode.Halfword
                                                 : SignExtensionInstruction.Opcode.Byte;

            return new SignExtensionInstruction(instructionPointer, ra, rs, rc, opcode);
        }

        private static GekkoInstruction ParseInstructionStoreInteger(string[] tokens, int instructionPointer)
        {
            var rs = ParseRegister(tokens[1]);

            var byteReverse = tokens[0].Contains("b");
            var updating    = tokens[0].Contains("u");
            var indexed     = tokens[0].Contains("x");

            // Ensures that rA isn't zero for updating variants
            Action<int> validateUpdatingVariantOperands = (src) => {
                if (src == 0)
                    throw new FormatException($"{tokens[0]} cannot have rA specified as r0");
            };

            if (indexed)
            {
                var ra = ParseRegister(tokens[2]);
                var rb = ParseRegister(tokens[3]);

                if (updating)
                    validateUpdatingVariantOperands(ra);

                // Halfword
                if (tokens[0].Contains("h"))
                {
                    var opcode = StoreHalfWordIndexedInstruction.Opcode.STHX;

                    if (updating)
                        opcode = StoreHalfWordIndexedInstruction.Opcode.STHUX;
                    else if (byteReverse)
                        opcode = StoreHalfWordIndexedInstruction.Opcode.STHBRX;

                    return new StoreHalfWordIndexedInstruction(instructionPointer, rs, ra, rb, opcode);
                }
                // Word
                else if (tokens[0].Contains("w"))
                {
                    var conditional = tokens[0].Contains("c");
                    var opcode = StoreWordIndexedInstruction.Opcode.STWX;

                    if (conditional)
                        opcode = StoreWordIndexedInstruction.Opcode.STWCX;
                    else if (updating)
                        opcode = StoreWordIndexedInstruction.Opcode.STWUX;
                    else if (byteReverse)
                        opcode = StoreWordIndexedInstruction.Opcode.STWBRX;

                    return new StoreWordIndexedInstruction(instructionPointer, rs, ra, rb, opcode);
                }
                else // Byte
                {
                    var opcode = updating ? StoreByteIndexedInstruction.Opcode.STBUX
                                          : StoreByteIndexedInstruction.Opcode.STBX;

                    return new StoreByteIndexedInstruction(instructionPointer, rs, ra, rb, opcode);
                }
            }
            else
            {
                var offset = ParseIntegerLiteral(tokens[2]);
                var ra     = ParseRegister(tokens[3]);

                if (updating)
                    validateUpdatingVariantOperands(ra);

                // Halfword
                if (tokens[0].Contains("h"))
                {
                    var opcode = updating ? StoreHalfWordInstruction.Opcode.STHU
                                          : StoreHalfWordInstruction.Opcode.STH;

                    return new StoreHalfWordInstruction(instructionPointer, rs, offset, ra, opcode);
                }
                // Word
                else if (tokens[0].Contains("w"))
                {
                    var opcode = updating ? StoreWordInstruction.Opcode.STWU
                                          : StoreWordInstruction.Opcode.STW;

                    return new StoreWordInstruction(instructionPointer, rs, offset, ra, opcode);
                }
                else // Byte
                {
                    var opcode = updating ? StoreByteInstruction.Opcode.STBU
                                          : StoreByteInstruction.Opcode.STB;

                    return new StoreByteInstruction(instructionPointer, rs, offset, ra, opcode);
                }
            }
        }

        private static GekkoInstruction ParseInstructionSUBF(string[] tokens, int instructionPointer)
        {
            var oe = tokens[0].Contains("o");
            var rc = tokens[0].Contains(".");

            var rd = ParseRegister(tokens[1]);
            var ra = ParseRegister(tokens[2]);
            var rb = ParseRegister(tokens[3]);

            // sub mnemonic
            if (!tokens[0].Contains("f"))
                return new SubtractFromInstruction(instructionPointer, rd, rb, ra, oe, rc);

            return new SubtractFromInstruction(instructionPointer, rd, ra, rb, oe, rc);
        }

        #endregion

        private static int ParseRegister(string register)
        {
            string literal;

            if (register == "sp")
                return 1;

            if (register.StartsWith("r"))
                literal = register.Substring(1);
            else if (register.StartsWith("fr"))
                literal = register.Substring(2);
            else
                throw new ArgumentException("Not a valid register", nameof(register));

            int registerNumber = ParseIntegerLiteral(literal);

            if (registerNumber < 0 || registerNumber > 31)
                throw new ArgumentException("General-purpose registers and floating-point registers must be within the range 0-31");

            return registerNumber;
        }

        private static int ParseConditionRegister(string register)
        {
            int registerNumber = ParseIntegerLiteral(register.Substring(3));

            if (registerNumber < 0 || registerNumber > 7)
                throw new ArgumentException("Condition registers must be within the range 0-7");

            return registerNumber;
        }

        private static int ParseSegmentRegister(string register)
        {
            if (!register.ToLower().StartsWith("sr"))
                throw new ArgumentException("Segment registers must be prefixed with 'sr' or 'SR'.");

            int registerNumber = ParseIntegerLiteral(register.Substring(2));

            if (registerNumber < 0 || registerNumber > 15)
                throw new ArgumentException("Segment registers must be within the range 0-15");

            return registerNumber;
        }

        private static int ParseSpecialPurposeRegister(string register)
        {
            switch (register.ToLower())
            {
                case "1":    // XER
                case "8":    // LR
                case "9":    // CTR
                case "18":   // DSISR
                case "19":   // DAR
                case "22":   // DEC
                case "25":   // SDR1
                case "26":   // SRR0
                case "27":   // SRR1
                case "272":  // SPRG0
                case "273":  // SPRG1
                case "274":  // SPRG2
                case "275":  // SPRG3
                case "282":  // EAR
                case "287":  // PVR
                case "528":  // IBAT0U
                case "529":  // IBAT0L
                case "530":  // IBAT1U
                case "531":  // IBAT1L
                case "532":  // IBAT2U
                case "533":  // IBAT2L
                case "534":  // IBAT3U
                case "535":  // IBAT3L
                case "536":  // DBAT0U
                case "537":  // DBAT0L
                case "538":  // DBAT1U
                case "539":  // DBAT1L
                case "540":  // DBAT2U
                case "541":  // DBAT2L
                case "542":  // DBAT3U
                case "543":  // DBAT3L
                case "912":  // GQR0
                case "913":  // GQR1
                case "914":  // GQR2
                case "915":  // GQR3
                case "916":  // GQR4
                case "917":  // GQR5
                case "918":  // GQR6
                case "919":  // GQR7
                case "920":  // HID2
                case "921":  // WPAR
                case "922":  // DMA_U
                case "923":  // DMA_L
                case "936":  // UMMCR0
                case "937":  // UPMC1
                case "938":  // UPMC2
                case "939":  // USIA
                case "940":  // UMMCR1
                case "941":  // UPMC3
                case "942":  // UPMC4
                case "943":  // USDA
                case "952":  // MMCR0
                case "953":  // PMC1
                case "954":  // PMC2
                case "955":  // SIA
                case "956":  // MMCR1
                case "957":  // PMC3
                case "958":  // PMC4
                case "959":  // SDA
                case "1008": // HID0
                case "1009": // HID1
                case "1010": // IABR
                case "1013": // DABR
                case "1017": // L2CR
                case "1019": // ICTC
                case "1020": // THRM1
                case "1021": // THRM2
                case "1022": // THRM3
                    return int.Parse(register);

                case "xer":    return 1;
                case "lr":     return 8;
                case "ctr":    return 9;
                case "dsisr":  return 18;
                case "dar":    return 19;
                case "dec":    return 22;
                case "sdr1":   return 25;
                case "srr0":   return 26;
                case "srr1":   return 27;
                case "sprg0":  return 272;
                case "sprg1":  return 273;
                case "sprg2":  return 274;
                case "sprg3":  return 275;
                case "ear":    return 282;
                case "pvr":    return 287;
                case "ibat0u": return 528;
                case "ibat0l": return 529;
                case "ibat1u": return 530;
                case "ibat1l": return 531;
                case "ibat2u": return 532;
                case "ibat2l": return 533;
                case "ibat3u": return 534;
                case "ibat3l": return 535;
                case "dbat0u": return 536;
                case "dbat0l": return 537;
                case "dbat1u": return 538;
                case "dbat1l": return 539;
                case "dbat2u": return 540;
                case "dbat2l": return 541;
                case "dbat3u": return 542;
                case "dbat3l": return 543;
                case "gqr0":   return 912;
                case "gqr1":   return 913;
                case "gqr2":   return 914;
                case "gqr3":   return 915;
                case "gqr4":   return 916;
                case "gqr5":   return 917;
                case "gqr6":   return 918;
                case "gqr7":   return 919;
                case "hid2":   return 920;
                case "wpar":   return 921;
                case "dma_u":  return 922;
                case "dma_l":  return 923;
                case "ummcr0": return 936;
                case "upmc1":  return 937;
                case "upmc2":  return 938;
                case "usia":   return 939;
                case "ummcr1": return 940;
                case "upmc3":  return 941;
                case "upmc4":  return 942;
                case "usda":   return 943;
                case "mmcr0":  return 952;
                case "pmc1":   return 953;
                case "pmc2":   return 954;
                case "sia":    return 955;
                case "mmcr1":  return 956;
                case "pmc3":   return 957;
                case "pmc4":   return 958;
                case "sda":    return 959;
                case "hid0":   return 1008;
                case "hid1":   return 1009;
                case "iabr":   return 1010;
                case "dabr":   return 1013;
                case "l2cr":   return 1017;
                case "ictc":   return 1019;
                case "thrm1":  return 1020;
                case "thrm2":  return 1021;
                case "thrm3":  return 1022;
            }

            throw new ArgumentException($"Invalid special-purpose register {register} specified.");
        }

        private static int ParseTimeBaseRegister(string register)
        {
            switch (register.ToLower())
            {
                case "268":
                case "269":
                    return int.Parse(register);

                case "tbl":
                    return 268;
                case "tbu":
                    return 269;
            }

            throw new ArgumentException("Invalid time base register. Must be TBL or TBU");
        }

        private static int ParseInstructionPointerLabel(string line)
        {
            var address = line.Remove(line.LastIndexOf(':'));
            return ParseIntegerLiteral(address);
        }

        private static long ParseInteger64Literal(string literal)
        {
            if (literal.Contains("0x"))
            {
                //Hexadecimal
                literal = literal.Replace("0x", "");
                var negative = literal.StartsWith("-");
                if (negative)
                    literal = literal.Substring(1);
                return (negative ? -1 : 1) * long.Parse(literal, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            else
            {
                return long.Parse(literal, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
        }

        private static int ParseIntegerLiteral(string literal)
        {
            return (int)ParseInteger64Literal(literal);
        }

        private static double ParseFloatLiteral(string literal)
        {
            return double.Parse(literal, CultureInfo.InvariantCulture);
        }

        private static string[] TokenizeLine(string line)
        {
            return Regex.Replace(line, "[\t,()]", " ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(token => token.Trim()).ToArray();
        }
    }
}
