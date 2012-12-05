using System.Collections.Generic;
using FubuCore.Dates;
using FubuMVC.SlickGrid;

namespace SlickGridHarness
{
    public class ConcertsSource : IGridDataSource<Concert>
    {
        public IEnumerable<Concert> GetData()
        {
            yield return new Concert
            {
                Band = "Billy Joe Shaver",
                Date = new Date(8, 4, 2012).Day,
                Location = "Antones",
                Genre = "Texas Country",
                Tour = "The Tour",
                TourScheduleUrl = "https://www.google.com"
            };

            yield return new Concert
            {
                Band = "Joe Ely Band",
                Date = new Date(8, 18, 2012).Day,
                Location = "Antones",
                Genre = "Texas Country",
                Tour = "The Tour",
                TourScheduleUrl = "https://www.google.com"
            };

            yield return new Concert
            {
                Band = "Charlie Robison",
                Date = new Date(9, 2, 2012).Day,
                Location = "Greune Hall",
                Genre = "Texas Country",
                Tour = "Texas Tour",
                TourScheduleUrl = "https://www.google.com"
            };

            yield return new Concert
            {
                Band = "Billy Joe Shaver",
                Date = new Date(9, 3, 2012).Day,
                Location = "Greune Hall",
                Genre = "Texas Country",
                Tour = "Texas Tour",
                TourScheduleUrl = "https://www.google.com"
            };

            yield return new Concert
            {
                Band = "Grace Potter and the Nocturnals",
                Date = new Date(11, 13, 2012).Day,
                Location = "Stubbs BBQ",
                Genre = "Adult Alternative",
                Tour = "NSFW Tour",
                TourScheduleUrl = "https://www.google.com"
            };
        }
    }
}