namespace GekkoAssembler
{
    public class BranchInstruction : IGekkoInstruction
    {
        public int Address { get; }
        public int ByteCode
        {
            get
            {
                return (18 << 26) | (0x3FFFFFC & (AA ? TargetAddress : (TargetAddress - Address))) | ((AA ? 1 : 0) << 1) | (LK ? 1 : 0);
            }
        }

        public int TargetAddress { get; }
        public bool AA { get; }
        public bool LK { get; }

        public BranchInstruction(int address, int targetAddress, bool aa = false, bool lk = false)
        {
            Address = address;
            TargetAddress = targetAddress;
            AA = aa;
            LK = lk;
        }
    }
}
