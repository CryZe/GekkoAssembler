namespace GekkoAssembler.Optimizers
{
    public interface IOptimizer
    {
        GekkoAssembly Optimize(GekkoAssembly assembly);
    }
}
