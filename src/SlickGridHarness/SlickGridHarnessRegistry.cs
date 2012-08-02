using FubuMVC.Core;

namespace SlickGridHarness
{
    public class SlickGridHarnessRegistry : FubuRegistry
    {
        public SlickGridHarnessRegistry()
        {
            Routes.HomeIs<HomeEndpoint>(x => x.Index());
        }
    }
}