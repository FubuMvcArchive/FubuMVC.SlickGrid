using System;
using FubuMVC.SlickGrid;

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
        public string Tour { get; set; }
        public string TourScheduleUrl { get; set; }
    }

    public class ConcertsGrid : GridDefinition<Concert>
    {
        public ConcertsGrid()
        {
            UsesHtmlConventions = true;
            SourceIs<ConcertsSource>();

            Column(x => x.Date);
            Column(x => x.Band).Frozen(true);
            Column(x => x.Location);
            Column(x => x.Genre);
            Column(x => x.Tour).Frozen(true);
            Column(x => x.TourScheduleUrl);

            Projection.Value(x => x.Url);
        }
    }
}