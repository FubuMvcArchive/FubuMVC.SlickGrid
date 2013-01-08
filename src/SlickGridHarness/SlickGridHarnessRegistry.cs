using FubuMVC.Core;

namespace SlickGridHarness
{
    public class SlickGridHarnessRegistry : FubuRegistry
    {
        public SlickGridHarnessRegistry()
        {
            Actions.FindBy(x =>
            {
                x.Applies.ToThisAssembly();
                x.IncludeClassesSuffixedWithEndpoint();
            });
        }
    }
}