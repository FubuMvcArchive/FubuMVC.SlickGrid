using System;
using FubuCore.Reflection;
using FubuMVC.Media.Projections;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class LambdaColumnRuleTester
    {
        [Test]
        public void matches_delegates()
        {
            var rule = new LambdaColumnRule(a => a.PropertyType == typeof (string),
                                            column => column.Properties["foo"] = "bar");

            var accessor1 = ReflectionHelper.GetAccessor<GridDefinitionTester.GridDefTarget>(x => x.Name);
            var accessor2 = ReflectionHelper.GetAccessor<GridDefinitionTester.GridDefTarget>(x => x.IsCool);

            rule.Matches(accessor1).ShouldBeTrue();
            rule.Matches(accessor2).ShouldBeFalse();
        }

        [Test]
        public void alteration_delegates()
        {
            var column = new ColumnDefinition<GridDefinition_selecting_editors_specs.GridDefTarget, String>(
                x => x.Name, new Projection<GridDefinition_selecting_editors_specs.GridDefTarget>());


            var rule = new LambdaColumnRule(a => a.PropertyType == typeof(string),
                                            c => c.Properties["foo"] = "bar");

            rule.Alter(column);

            column.Property("foo").ShouldEqual("bar");
        }
    }
}