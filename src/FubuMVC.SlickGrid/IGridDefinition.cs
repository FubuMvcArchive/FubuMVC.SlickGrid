using System.Collections.Generic;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.SlickGrid
{
    public interface IGridDefinition<T> : IGridDefinition
    {
        Projection<T> Projection { get; }
    }

    public interface IGridDefinition
    {
        string ToColumnJson();
        string SelectDataSourceUrl(IUrlRegistry urls);
    }

    
}