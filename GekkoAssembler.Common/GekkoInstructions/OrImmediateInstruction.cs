namespace GekkoAssembler.GekkoInstructions
{
    public class OrImmediateInstruction : GekkoInstruction
    {
        public override int ByteCode
            => (((((24 << 5) | RegisterSource) << 5) | RegisterA) << 16) | UIMM;

        public int RegisterA { get; }
        public int RegisterSource { get; }
        public int UIMM { get; }

        public OrImmediateInstruction(int address, int registerA, int registerSource, int uimm) : base(address)
        {
            RegisterA = registerA;
            RegisterSource = registerSource;
            UIMM = uimm;
        }
    }
}
