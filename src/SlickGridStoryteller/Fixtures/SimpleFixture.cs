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
        private IWebElement _editor;

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

        [FormatAs("The concert date for {band} playing at {location} should be {date}")]
        public string TheDateIs(string band, string location)
        {
            return Driver.GridAction<Concert>("concertGrid")
                .Where(x => x.Band).Is(band)
                .Where(x => x.Location).Is(location)
                .TextFor(x => x.Date);
        }

        [FormatAs("Edit the genre for the band {band}")]
        public void EditGenreForBand(string band)
        {
            _editor = Driver.GridAction<Concert>("concertGrid")
                .Where(x => x.Band).Is(band)
                .Editor(x => x.Genre);
        }

        [FormatAs("The value of the genre in the editor should be {genre}")]
        public string TheEditorText()
        {
            return Serenity.Fixtures.Handlers.ElementHandlers.FindHandler(_editor).GetData(Driver, _editor);
        }

        [FormatAs("Change the genre to {genre}")]
        public void ChangeGenre(string genre)
        {
            Serenity.Fixtures.Handlers.ElementHandlers.FindHandler(_editor).EnterData(Driver, _editor, genre);
        
            _editor.SendKeys(Keys.Tab);
        }


    }
}