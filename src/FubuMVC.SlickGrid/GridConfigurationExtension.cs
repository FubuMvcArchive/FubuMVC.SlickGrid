using System;
using FubuMVC.Core;
using FubuCore;
using System.Collections.Generic;

namespace FubuMVC.SlickGrid
{
    public class GridConfigurationExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            // Some ugly Generic trickery
            registry.WithTypes(types => types.TypesMatching(IsGridDefinitionType).Each(type =>
            {
                typeof(Loader<>).CloseAndBuildAs<ILoader>(type).Apply(registry);
            }));
        }

        public interface ILoader
        {
            void Apply(FubuRegistry registry);
        }

        public class Loader<T> : ILoader where T : IFubuRegistryExtension, new()
        {
            public void Apply(FubuRegistry registry)
            {
                registry.Import<T>();
            }
        }

        public static bool IsGridDefinitionType(Type type)
        {
            return type.IsConcrete() && type.CanBeCastTo<IGridDefinition>() && type.CanBeCastTo<IFubuRegistryExtension>();
        }
    }
}