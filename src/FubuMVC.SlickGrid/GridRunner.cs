using System.Collections.Generic;
using FubuMVC.Core.UI.Security;
using FubuMVC.Media.Projections;
using System.Linq;

namespace FubuMVC.SlickGrid
{


    // Letting this be covered by more integrated tests
    public class GridRunner<T, TGrid, TDataSource>
        where TGrid : IGridDefinition<T>
        where TDataSource : IGridDataSource<T>
    {
        private readonly TGrid _grid;
        private readonly TDataSource _source;
        private readonly IProjectionRunner<T> _runner;
        private readonly IFieldAccessService _accessService;

        public GridRunner(TGrid grid, TDataSource source, IProjectionRunner<T> runner, IFieldAccessService accessService)
        {
            _grid = grid;
            _source = source;
            _runner = runner;
            _accessService = accessService;
        }

        public IDictionary<string, object> Run()
        {
            var data = _source.GetData();
            var projection = _grid.ToProjection(_accessService);

            var results = data.Select(x =>
            {
                return _runner.ProjectToJson(projection, new SimpleValues<T>(x));
            }).ToArray();

            return new Dictionary<string, object>
            {
                {"data", results}
            };
        }
    }

    public class GridRunner<T, TGrid, TDataSource, TQuery>
        where TGrid : IGridDefinition<T>
        where TDataSource : IGridDataSource<T, TQuery>
    {
        private readonly TGrid _grid;
        private readonly TDataSource _source;
        private readonly IProjectionRunner<T> _runner;
        private readonly IFieldAccessService _accessService;

        public GridRunner(TGrid grid, TDataSource source, IProjectionRunner<T> runner, IFieldAccessService accessService)
        {
            _grid = grid;
            _source = source;
            _runner = runner;
            _accessService = accessService;
        }

        public IDictionary<string, object> Run(TQuery query)
        {
            var data = _source.GetData(query);
            var projection = _grid.ToProjection(_accessService);

            var results = data.Select(x =>
            {
                return _runner.ProjectToJson(projection, new SimpleValues<T>(x));
            }).ToArray();

            return new Dictionary<string, object>
            {
                {"data", results}
            };
        }
    }
}