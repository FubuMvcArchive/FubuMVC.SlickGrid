using System;
using FubuMVC.Core;
using FubuCore;
using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.SlickGrid
{
    public class GridConfigurationExtension : IFubuRegistryExtension
    {
        #region IFubuRegistryExtension Members

        public void Configure(FubuRegistry registry)
        {
            var types = new TypePool();
            types.AddAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            types.IgnoreExportTypeFailures = true;

            // Some ugly Generic trickery
            types.TypesMatching(IsGridDefinitionType).Each(type => {
                typeof (Loader<>).CloseAndBuildAs<ILoader>(type).Apply(registry);
            });


            var policies = new ColumnPolicies();
            registry.ReplaceSettings(policies);

            registry.Services(x => x.AddService<IColumnPolicies>(policies));
        }

        #endregion

        public static bool IsGridDefinitionType(Type type)
        {
            return type.IsConcrete() && type.CanBeCastTo<IGridDefinition>() &&
                   type.CanBeCastTo<IFubuRegistryExtension>();
        }

        #region Nested type: ILoader

        public interface ILoader
        {
            void Apply(FubuRegistry registry);
        }

        #endregion

        #region Nested type: Loader

        public class Loader<T> : ILoader where T : IFubuRegistryExtension, new()
        {
            #region ILoader Members

            public void Apply(FubuRegistry registry)
            {
                registry.Import<T>();
            }

            #endregion
        }

        #endregion
    }
}