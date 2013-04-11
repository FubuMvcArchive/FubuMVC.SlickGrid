using System.Collections.Generic;
using System.Linq;
using FubuMVC.Media.Projections;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class PagedGridRunnerTester
    {
        [Test]
        public void run_with_query()
        {
            var runner = new PagedGridRunner<Item, ItemGrid, ItemSource, ItemQuery>(new ItemGrid(), new ItemSource(), new ProjectionRunner<Item>(new ProjectionRunner(new InMemoryServiceLocator())), new StubFieldAccessService());

            var result = runner.Run(new ItemQuery {page = 3});
            result["pageCount"].ShouldEqual(20); // 500 records, 25 per page

            var dicts = result["data"].As<IEnumerable<IDictionary<string, object>>>().ToArray().As<IDictionary<string, object>[]>();
            dicts.Length.ShouldEqual(ItemSource.PageSize);
        }
    }
}