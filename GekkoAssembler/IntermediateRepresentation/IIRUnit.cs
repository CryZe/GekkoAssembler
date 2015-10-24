namespace GekkoAssembler
{
    public interface IIRUnit
    {
        void Accept(IIRUnitVisitor visitor);
    }
}