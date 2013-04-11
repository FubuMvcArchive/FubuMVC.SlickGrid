using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.UI.Security;
using FubuMVC.Media.Projections;

namespace FubuMVC.SlickGrid
{
    public class PagedQuery
    {
        public int page { get; set; }
    }

    public class PagedResults<T>
    {
        public int PageCount { get; set; }
        public IEnumerable<T> Data { get; set; }
    }

    public interface IPagedGridDataSource<T, TQuery> where TQuery : PagedQuery
    {
        PagedResults<T> GetData(TQuery query);
    }

    public class PagedGridRunner<T, TGrid, TDataSource, TQuery>
        where TGrid : IGridDefinition<T>
        where TDataSource : IPagedGridDataSource<T, TQuery> where TQuery : PagedQuery
    {
        private readonly TGrid _grid;
        private readonly TDataSource _source;
        private readonly IProjectionRunner<T> _runner;
        private readonly IFieldAccessService _accessService;

        public PagedGridRunner(TGrid grid, TDataSource source, IProjectionRunner<T> runner, IFieldAccessService accessService)
        {
            _grid = grid;
            _source = source;
            _runner = runner;
            _accessService = accessService;
        }

        // Will need to add these as json
        public IDictionary<string, object> Run(TQuery query)
        {
            var projection = _grid.ToProjection(_accessService);

            return new Dictionary<string, object>
            {
                {"data", _source.GetData(query).Data.Select(x => _runner.ProjectToJson(projection, new SimpleValues<T>(x)))}, 
                {"pageCount", _source.GetData(query).PageCount}
            };
        }
    }
}