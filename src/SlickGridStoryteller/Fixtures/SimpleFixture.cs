using OpenQA.Selenium;
using Serenity.Fixtures;
using SlickGridHarness;
using SlickGridHarness.Simple;
using StoryTeller.Engine;
using FubuMVC.SlickGrid.Serenity;

namespace SlickGridStoryteller.Fixtures
{
    public class SimpleFixture : ScreenFixture
    {
        protected override void beforeRunning()
        {
            Navigation.NavigateTo<SimpleEndpoint>(x => x.get_simple());
        }

        [FormatAs("Click on the row with band {band}")]
        public void ClickBand(string band)
        {
            Driver.GridAction<Concert>("concertGrid")
                .Where(x => x.Band).Is(band).ClickOnRow();
        }

        [FormatAs("The band name selected is {band}")]
        public string TheSelectedBandIs()
        {
            return Driver.FindElement(By.Id("band")).Text;
        }
    }
}