using OpenQA.Selenium;
using Serenity.Fixtures;
using SlickGridHarness;
using SlickGridHarness.Simple;
using StoryTeller.Engine;
using FubuMVC.SlickGrid.Serenity;

namespace SlickGridStoryteller.Fixtures
{
    public class SimpleFixture : SlickGridFixture<Concert>
    {
        private IWebElement _editor;

        public SimpleFixture() : base("concertGrid")
        {
        }

        protected override void beforeRunning()
        {
            Navigation.NavigateTo<SimpleEndpoint>(x => x.get_simple());
        }

        [FormatAs("Click on the row with band {band}")]
        public void ClickBand(string band)
        {
            Where(x => x.Band).Is(band).ClickOnRow();
        }

        [FormatAs("The band name selected is {band}")]
        public string TheSelectedBandIs()
        {
            return Driver.FindElement(By.Id("band")).Text;
        }

        [FormatAs("The concert date for {band} playing at {location} should be {date}")]
        public string TheDateIs(string band, string location)
        {
            return Where(x => x.Band).Is(band)
                .And(x => x.Location).Is(location)
                .TextFor(x => x.Date);
        }

        [FormatAs("Edit the genre for the band {band}")]
        public void EditGenreForBand(string band)
        {
            _editor = Where(x => x.Band).Is(band).Editor(x => x.Genre);
        }

        [FormatAs("The value of the genre in the editor should be {genre}")]
        public string TheEditorText()
        {
            return GetData(_editor);
        }

        [FormatAs("Change the genre to {genre}")]
        public void ChangeGenre(string genre)
        {
            SetData(_editor, genre);
        
            _editor.SendKeys(Keys.Tab);
        }


    }
}