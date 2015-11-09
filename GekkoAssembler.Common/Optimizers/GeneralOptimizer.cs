namespace GekkoAssembler.Optimizers
{
    public class GeneralOptimizer : CompositeOptimizer
    {
        public GeneralOptimizer()
            : base(new IRMultiUnitOptimizer(), 
                  new WriteDataOptimizer())
        { }
    }
}
