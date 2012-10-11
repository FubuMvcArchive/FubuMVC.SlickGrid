using FubuMVC.Core;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class column_policies_is_registered
    {
        [Test]
        public void the_policies_from_settings_are_available()
        {
            var registry = new FubuRegistry();
            new GridConfigurationExtension().Configure(registry);

            var graph = BehaviorGraph.BuildFrom(registry);

            var policies = graph.Settings.Get<ColumnPolicies>();

            graph.Services.DefaultServiceFor<IColumnPolicies>().Value.ShouldBeTheSameAs(policies);
        }
    }
}