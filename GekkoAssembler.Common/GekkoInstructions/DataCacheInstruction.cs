namespace GekkoAssembler.GekkoInstructions
{
    public sealed class DataCacheInstruction : GekkoInstruction
    {
        public enum PrimaryOpcode
        {
            // Every data cache instruction but dcbz_l has 31 as the primary opcode.
            NonDCBZ_L = 31,
            DCBZ_L    = 4
        }

        public enum SecondaryOpcode
        {
            DCBF   = 86,
            DCBI   = 470,
            DCBST  = 54,
            DCBT   = 278,
            DCBTST = 246,
            DCBZ   = 1014,
            DCBZ_L = 1014
        }

        public override int ByteCode { get; }

        public DataCacheInstruction(int address, PrimaryOpcode primary, SecondaryOpcode secondary, int rA, int rB) : base(address)
        {
            ByteCode = ((int)primary << 26 | rA << 16 | rB << 11 | (int)secondary << 1);
        }
    }
}
