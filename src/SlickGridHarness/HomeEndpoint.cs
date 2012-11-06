using System;
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

    public class ConcertsGrid : GridDefinition<Concert>
    {
        public ConcertsGrid()
        {
            UsesHtmlConventions = true;
            SourceIs<ConcertsSource>();

            Column(x => x.Date);
            Column(x => x.Band);
            Column(x => x.Location);
            Column(x => x.Genre);

            Projection.Value(x => x.Url);
        }
    }
}