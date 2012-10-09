using System.Collections.Generic;
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

        public GridRunner(TGrid grid, TDataSource source, IProjectionRunner<T> runner)
        {
            _grid = grid;
            _source = source;
            _runner = runner;
        }

        public IEnumerable<IDictionary<string, object>> Run()
        {
            var data = _source.GetData();
            return data.Select(x => {
                return _runner.ProjectToJson(_grid.Projection, new SimpleValues<T>(x));
            });
        }
    }

    public class GridRunner<T, TGrid, TDataSource, TQuery>
        where TGrid : IGridDefinition<T>
        where TDataSource : IGridDataSource<T, TQuery>
    {
        private readonly TGrid _grid;
        private readonly TDataSource _source;
        private readonly IProjectionRunner<T> _runner;

        public GridRunner(TGrid grid, TDataSource source, IProjectionRunner<T> runner)
        {
            _grid = grid;
            _source = source;
            _runner = runner;
        }

        public IEnumerable<IDictionary<string, object>> Run(TQuery query)
        {
            var data = _source.GetData(query);
            return data.Select(x =>
            {
                return _runner.ProjectToJson(_grid.Projection, new SimpleValues<T>(x));
            });
        }
    }
}