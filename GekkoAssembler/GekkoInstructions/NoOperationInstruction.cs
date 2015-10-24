namespace GekkoAssembler
{
    public class NoOperationInstruction : OrImmediateInstruction
    {
        public NoOperationInstruction(int address)
            : base(address, 0, 0, 0)
        { }
    }
}
