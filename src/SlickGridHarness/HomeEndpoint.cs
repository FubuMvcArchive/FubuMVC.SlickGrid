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
        private readonly FubuHtmlDocument<HomeModel> _document;

        public HomeEndpoint(FubuHtmlDocument<HomeModel> document)
        {
            _document = document;
        }

        public HtmlDocument Index()
        {
            _document.Title = "FubuMVC.SlickGrid Demonstration";
            _document.Add("h1").Text("FubuMVC.SlickGrid Demonstration");

            _document.Add(doc => doc.RenderGrid<ConcertsGrid>("concertGrid").Attr("style", "width:100%;height:500px;"));

            _document.WriteAssetsToHead();

            return _document;
        }
    }

    public class HomeModel
    {
    }

    public class Concert
    {
        public DateTime Date { get; set; }
        public string Band { get; set; }
        public string Location { get; set; }
    }

    public class ConcertsSource : IGridDataSource<Concert>
    {
        public IEnumerable<Concert> GetData()
        {
            yield return new Concert{
                Band = "Billy Joe Shaver",
                Date = new Date(8, 4, 2012).Day,
                Location = "Antones"
            };

            yield return new Concert{
                Band = "Joe Ely Band",
                Date = new Date(8, 18, 2012).Day,
                Location = "Antones"
            };

            yield return new Concert{
                Band = "Charlie Robison",
                Date = new Date(9, 2, 2012).Day,
                Location = "Greune Hall"
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
        }
    }
}