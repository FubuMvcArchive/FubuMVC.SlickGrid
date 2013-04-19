using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Templates;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class GridRunnerTester
    {
        [Test]
        public void run_with_no_query()
        {
            var runner = new GridRunner<Foo, FooGrid, FooSource>(new FooGrid(), new FooSource(), new ProjectionRunner<Foo>(new ProjectionRunner(new InMemoryServiceLocator())), new StubFieldAccessService());
            var result = runner.Run()["data"];
            var dicts = result.As<IDictionary<string, object>[]>();

            dicts.Select(x => x["name"]).ShouldHaveTheSameElementsAs("Scooby", "Shaggy", "Velma");
        }

        [Test]
        public void run_with_query()
        {
            var runner = new GridRunner<Foo, FooGrid, FancyFooSource, FooQuery>(new FooGrid(), new FancyFooSource(), new ProjectionRunner<Foo>(new ProjectionRunner(new InMemoryServiceLocator())), new StubFieldAccessService());
            var dicts = runner.Run(new FooQuery { Letter = "S" })["data"].As<IDictionary<string, object>[]>(); ;

            dicts.Select(x => x["name"]).ShouldHaveTheSameElementsAs("Scooby", "Shaggy");
        }
    }

    public class FooQuery
    {
        public string Letter { get; set; }
    }

    public class FancyFooSource : IGridDataSource<Foo, FooQuery>
    {
        public IEnumerable<Foo> GetData(FooQuery query)
        {
            return new FooSource().GetData().Where(x => x.Name.StartsWith(query.Letter));
        }
    }

    public class FooSource : IGridDataSource<Foo>
    {
        public IEnumerable<Foo> GetData()
        {
            yield return new Foo{
                Name = "Scooby"
            };

            yield return new Foo
            {
                Name = "Shaggy"
            };

            yield return new Foo(){
                Name = "Velma"
            };
        }
    }

    public class FooGrid : IGridDefinition<Foo>
    {
        public Projection<Foo> ToProjection(IFieldAccessService accessService)
        {
            return Projection;
        }

        IEnumerable<IGridColumn> IGridDefinition.Columns()
        {
            throw new NotImplementedException();
        }

        public Type DetermineRunnerType()
        {
            throw new NotImplementedException();
        }

        public string ToColumnJson(IFieldAccessService accessService)
        {
            throw new NotImplementedException();
        }

        public string SelectDataSourceUrl(IUrlRegistry urls)
        {
            throw new NotImplementedException();
        }

        public bool UsesHtmlConventions { get; set; }
        public void SelectFormattersAndEditors(IColumnPolicies editors)
        {
            throw new NotImplementedException();
        }

        public void WriteAnyTemplates(ITemplateWriter writer)
        {
            throw new NotImplementedException();
        }

        public bool AllColumnsAreEditable { get; set; }
        public SlickGridFormatter DefaultFormatter { get; set; }
        public bool IsPaged()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGridColumn> Columns { get; private set; }

        public Projection<Foo> Projection { get
        {
            var projection = new Projection<Foo>();
            projection.Value(x => x.Name).Name("name");

            return projection;
        }
        }
    }

    public class Foo
    {
        public string Name { get; set; }
    }
}