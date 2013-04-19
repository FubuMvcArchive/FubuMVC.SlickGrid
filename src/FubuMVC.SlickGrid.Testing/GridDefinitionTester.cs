using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class GridDefinitionTester
    {
        public class TargetGrid : GridDefinition<GridDefTarget>
        {
            public TargetGrid()
            {

            }
        }

        public class TargetSource : IGridDataSource<GridDefTarget>
        {
            public IEnumerable<GridDefTarget> GetData()
            {
                throw new NotImplementedException();
            }
        }

        public class TargetGridWithColumns : GridDefinition<GridDefTarget>
        {
            public TargetGridWithColumns()
            {
                Column(c => c.Name);
                Column(c => c.IsCool);
            }
        }

        [Test]
        public void is_paged_false_with_non_paged_source()
        {
            var grid = new TargetGrid();
            grid.SourceIs<SimpleGoodSource>();

            grid.IsPaged().ShouldBeFalse();
        }

        [Test]
        public void is_paged_true_with_paged_source()
        {
            var grid = new TargetGrid();
            grid.SourceIs<PagedSource>();

            grid.IsPaged().ShouldBeTrue();
        }

        [Test]
        public void projection_does_not_include_not_authorized_columns()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Column(x => x.Name);
            grid.Column(x => x.IsCool);

            var service = new StubFieldAccessService();
            service.SetRights<GridDefTarget>(x => x.Count, AccessRight.ReadOnly);
            service.SetRights<GridDefTarget>(x => x.Name, AccessRight.None);

            var projection = grid.ToProjection(service);

            projection.As<IProjection<GridDefTarget>>().Accessors()
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Count", "IsCool");
        }

        [Test]
        public void create_column_json()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Column(x => x.IsCool);
            grid.Column(x => x.Name);

            var json = grid.As<IGridDefinition>().ToColumnJson(new StubFieldAccessService());

            json
                .ShouldEqual("[{name: \"en-US_Count\", field: \"Count\", id: \"Count\", sortable: true, frozen: false}, {name: \"en-US_IsCool\", field: \"IsCool\", id: \"IsCool\", sortable: true, frozen: false}, {name: \"en-US_Name\", field: \"Name\", id: \"Name\", sortable: true, frozen: false}]");
        }

        [Test]
        public void create_frozen_column_first_json()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Column(x => x.IsCool).Frozen(true);
            grid.Column(x => x.Name);

            var json = grid.As<IGridDefinition>().ToColumnJson(new StubFieldAccessService());

            json
                .ShouldEqual("[{name: \"en-US_IsCool\", field: \"IsCool\", id: \"IsCool\", sortable: true, frozen: true}, {name: \"en-US_Count\", field: \"Count\", id: \"Count\", sortable: true, frozen: false}, {name: \"en-US_Name\", field: \"Name\", id: \"Name\", sortable: true, frozen: false}]");
        }

        [Test]
        public void create_frozen_columns_first_json()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Column(x => x.IsCool).Frozen(true);
            grid.Column(x => x.Name).Frozen(true);

            var json = grid.As<IGridDefinition>().ToColumnJson(new StubFieldAccessService());

            json
                .ShouldEqual("[{name: \"en-US_IsCool\", field: \"IsCool\", id: \"IsCool\", sortable: true, frozen: true}, {name: \"en-US_Name\", field: \"Name\", id: \"Name\", sortable: true, frozen: true}, {name: \"en-US_Count\", field: \"Count\", id: \"Count\", sortable: true, frozen: false}]");
        }

        [Test]
        public void create_all_frozen_columns_json()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count).Frozen(true);
            grid.Column(x => x.IsCool).Frozen(true);
            grid.Column(x => x.Name).Frozen(true);

            var json = grid.As<IGridDefinition>().ToColumnJson(new StubFieldAccessService());

            json
                .ShouldEqual("[{name: \"en-US_Count\", field: \"Count\", id: \"Count\", sortable: true, frozen: true}, {name: \"en-US_IsCool\", field: \"IsCool\", id: \"IsCool\", sortable: true, frozen: true}, {name: \"en-US_Name\", field: \"Name\", id: \"Name\", sortable: true, frozen: true}]");
        }

        [Test]
        public void create_column_json_with_data_elements()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Data(x => x.IsCool);
            grid.Column(x => x.Name);

            var json = grid.As<IGridDefinition>().ToColumnJson(new StubFieldAccessService());

            json
                .ShouldEqual("[{name: \"en-US_Count\", field: \"Count\", id: \"Count\", sortable: true, frozen: false}, {name: \"en-US_Name\", field: \"Name\", id: \"Name\", sortable: true, frozen: false}]");
        }

        [Test]
        public void create_column_json_with_authorization_field_rights()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Data(x => x.IsCool);
            grid.Column(x => x.Name);
            grid.Column(x => x.Random);

            var service = new StubFieldAccessService();
            service.SetRights<GridDefTarget>(x => x.Random, AccessRight.None);
            service.SetRights<GridDefTarget>(x => x.Name, AccessRight.ReadOnly);

            var json = grid.As<IGridDefinition>().ToColumnJson(service);

            json
                .ShouldEqual("[{name: \"en-US_Count\", field: \"Count\", id: \"Count\", sortable: true, frozen: false}, {name: \"en-US_Name\", field: \"Name\", id: \"Name\", sortable: true, frozen: false}]");
        }


        [Test]
        public void format_data_smoke_test()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Column(x => x.IsCool);
            grid.Column(x => x.Name);

            var data = new GridDefTarget[]{
                new GridDefTarget{Count = 1, IsCool = true, Name = "Scooby"},
                new GridDefTarget{Count = 2, IsCool = true, Name = "Velma"},
                new GridDefTarget{Count = 3, IsCool = true, Name = "Daphne"},
            };

            IProjection<GridDefTarget> projection = grid.As<IGridDefinition<GridDefTarget>>().ToProjection(new StubFieldAccessService()).As<IProjection<GridDefTarget>>();
            

            var dicts = data.Select(x => {
                var node = new DictionaryMediaNode();

                projection.Write(new ProjectionContext<GridDefTarget>(new InMemoryServiceLocator(), x), node);

                return node.Values;
            });

            dicts.Select(x => x["Name"]).ShouldHaveTheSameElementsAs("Scooby", "Velma", "Daphne");
            dicts.Select(x => x["Count"]).ShouldHaveTheSameElementsAs(1, 2, 3);
        }

        [Test]
        public void throws_if_source_cannot_work_1()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TargetGrid().SourceIs<NotASource>();
            });
        }

        [Test]
        public void throws_if_source_cannot_work_2()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TargetGrid().SourceIs<WrongSource>();
            });
        }

        [Test]
        public void throws_if_source_cannot_work_3()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TargetGrid().SourceIs<MoreWrongSource>();
            });
        }

        [Test]
        public void source_happy_path_without_query_object()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<SimpleGoodSource>();

            targetGrid.SourceType.ShouldEqual(typeof (SimpleGoodSource));
        }

        [Test]
        public void source_happy_path_with_query_object()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<QueryGoodSource>();

            targetGrid.SourceType.ShouldEqual(typeof (QueryGoodSource));
        }

        [Test]
        public void select_data_source_url_with_no_data_source()
        {
            new TargetGrid().As<IGridDefinition>().SelectDataSourceUrl(new StubUrlRegistry())
                .ShouldBeNull();
        }

        [Test]
        public void select_data_source_url_with_source_type_and_no_query()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<SimpleGoodSource>();

            targetGrid.As<IGridDefinition>().DetermineRunnerType().ShouldEqual(typeof(GridRunner<GridDefTarget, TargetGrid,SimpleGoodSource>));
        }

        [Test]
        public void select_data_source_url_with_source_type_and_a_query()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<QueryGoodSource>();

            targetGrid.As<IGridDefinition>().DetermineRunnerType().ShouldEqual(typeof(GridRunner<GridDefTarget, TargetGrid, QueryGoodSource, DifferentClass>));
        }

        [Test]
        public void select_data_source_url_with_paged_source()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<PagedSource>();

            targetGrid.As<IGridDefinition>().DetermineRunnerType().ShouldEqual(typeof(PagedGridRunner<GridDefTarget, TargetGrid, PagedSource, SpecialPagedQuery>));
        }

        [Test]
        public void exposes_available_columns()
        {
            IGridDefinition targetGrid = new TargetGridWithColumns();
            targetGrid.Columns().Count().ShouldEqual(2);
        }


        public class NotASource{}
        public class WrongSource : IGridDataSource<DifferentClass>
        {
            public IEnumerable<DifferentClass> GetData()
            {
                throw new NotImplementedException();
            }
        }

        public class MoreWrongSource : IGridDataSource<DifferentClass, DifferentClass>
        {
            public IEnumerable<DifferentClass> GetData(DifferentClass query)
            {
                throw new NotImplementedException();
            }
        }

        public class SpecialPagedQuery : PagedQuery
        {
            
        }

        public class PagedSource : IPagedGridDataSource<GridDefTarget, SpecialPagedQuery>
        {
            public PagedResults<GridDefTarget> GetData(SpecialPagedQuery query)
            {
                throw new NotImplementedException();
            }
        }

        public class SimpleGoodSource : IGridDataSource<GridDefTarget>
        {
            public IEnumerable<GridDefTarget> GetData()
            {
                throw new NotImplementedException();
            }
        }

        public class QueryGoodSource : IGridDataSource<GridDefTarget, DifferentClass>
        {
            public IEnumerable<GridDefTarget> GetData(DifferentClass query)
            {
                throw new NotImplementedException();
            }
        }


        public class DifferentClass{}

        public class GridDefTarget
        {
            public string Name { get; set; }
            public bool IsCool { get; set; }
            public int Count { get; set; }

            public string Random { get; set; }
        }
    }

    public class StubFieldAccessService : IFieldAccessService
    {
        private readonly Cache<Accessor, AccessRight> _rights = new Cache<Accessor, AccessRight>(a => AccessRight.All); 

        public void SetRights<T>(Expression<Func<T, object>> property, AccessRight rights)
        {
            var accessor = property.ToAccessor();
            _rights[accessor] = rights;
        }

        public AccessRight RightsFor(ElementRequest request)
        {
            return RightsFor(null, request.Accessor.InnerProperty);
        }

        public AccessRight RightsFor(object target, PropertyInfo property)
        {
            var accessor = new SingleProperty(property);
            return _rights[accessor];
        }
    }
}