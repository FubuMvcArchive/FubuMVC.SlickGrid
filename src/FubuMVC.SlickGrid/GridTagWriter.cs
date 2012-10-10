using System;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.SlickGrid
{
    public class GridTagWriter<T> where T : IGridDefinition
    {
        public GridTagWriter(IAssetRequirements assets, IUrlRegistry urls, T grid)
        {
            
        } 

        public HtmlTag Write(string id)
        {
            throw new NotImplementedException();
        }
    }

    
}