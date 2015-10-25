namespace GekkoAssembler.GekkoInstructions
{
    public class BranchToLinkRegisterInstruction : GekkoInstruction
    {
        public override int Address { get; }
        public override int ByteCode => 0x4E800020;

        public BranchToLinkRegisterInstruction(int address)
        {
            Address = address;
        }
    }
}
