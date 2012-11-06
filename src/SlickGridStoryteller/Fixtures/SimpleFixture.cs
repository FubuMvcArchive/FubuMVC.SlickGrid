using Serenity.Fixtures;
using SlickGridHarness.Simple;

namespace SlickGridStoryteller.Fixtures
{
    public class SimpleFixture : ScreenFixture
    {
        protected override void beforeRunning()
        {
            Navigation.NavigateTo<SimpleEndpoint>(x => x.get_simple());
        }
    }
}