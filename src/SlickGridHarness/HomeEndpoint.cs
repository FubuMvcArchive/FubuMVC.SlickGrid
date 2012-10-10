using System;
using System.Collections.Generic;
using FubuCore.Dates;
using FubuMVC.Core.UI;
using FubuMVC.SlickGrid;
using HtmlTags;

namespace SlickGridHarness
{
    public class HomeEndpoint
    {
        public HomeViewModel Index()
        {
            return new HomeViewModel();
        }

        public HomeViewModel get_another()
        {
            return new HomeViewModel();
        }
    }

    public class HomeViewModel
    {
    }

    public class Concert
    {
        public Concert()
        {
            Url = Guid.NewGuid().ToString();
        }

        public DateTime Date { get; set; }
        public string Band { get; set; }
        public string Location { get; set; }
        public string Genre { get; set; }

        public string Url { get; set; }
    }

    public class ConcertsSource : IGridDataSource<Concert>
    {
        public IEnumerable<Concert> GetData()
        {
            yield return new Concert{
                Band = "Billy Joe Shaver",
                Date = new Date(8, 4, 2012).Day,
                Location = "Antones",
                Genre = "Texas Country"
            };

            yield return new Concert{
                Band = "Joe Ely Band",
                Date = new Date(8, 18, 2012).Day,
                Location = "Antones",
                Genre = "Texas Country"
            };

            yield return new Concert{
                Band = "Charlie Robison",
                Date = new Date(9, 2, 2012).Day,
                Location = "Greune Hall",
                Genre = "Texas Country"
            };
            yield return new Concert{
                Band = "Grace Potter and the Nocturnals",
                Date = new Date(11, 13, 2012).Day,
                Location = "Stubbs BBQ",
                Genre = "Adult Alternative"
            };
        }
    }

    public class ConcertsGrid : GridDefinition<Concert>
    {
        public ConcertsGrid()
        {
            SourceIs<ConcertsSource>();

            Column(x => x.Date);
            Column(x => x.Band);
            Column(x => x.Location);
            Column(x => x.Genre);

            Projection.Value(x => x.Url);
        }
    }
}