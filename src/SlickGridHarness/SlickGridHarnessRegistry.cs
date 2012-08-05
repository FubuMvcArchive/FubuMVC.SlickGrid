using FubuMVC.Core;
using FubuMVC.Spark;

namespace SlickGridHarness
{
    public class SlickGridHarnessRegistry : FubuRegistry
    {
        public SlickGridHarnessRegistry()
        {
            Routes.HomeIs<HomeEndpoint>(x => x.Index());
            Views.TryToAttachWithDefaultConventions();
        }
    }
}