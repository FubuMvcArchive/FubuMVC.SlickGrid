using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Assets;
using FubuMVC.Core.UI.Templates;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.SlickGrid
{


    public class GridTagWriter<T> where T : IGridDefinition
    {
        private readonly IColumnPolicies _policies;
        private readonly ITemplateWriter _templates;
        private readonly IAssetRequirements _assets;
        private readonly IUrlRegistry _urls;
        private readonly T _grid;

        public GridTagWriter(IColumnPolicies policies, ITemplateWriter templates, IAssetRequirements assets, IUrlRegistry urls, T grid)
        {
            _policies = policies;
            _templates = templates;
            _assets = assets;
            _urls = urls;
            _grid = grid;
        }

        public HtmlTag Write(string id)
        {
            _assets.Require("slickgrid_styles", "slickgrid/SlickGridActivator.js");
            if (_grid.UsesHtmlConventions)
            {
                _assets.Require("underscore", "slickgrid/slickGridTemplates.js");
            }

            _grid.SelectFormattersAndEditors(_policies);
            _grid.WriteAnyTemplates(_templates);

            var div = new HtmlTag("div").Id(id).AddClass("slick-grid");
            div.Data("columns", _grid.ToColumnJson());
            var url = _grid.SelectDataSourceUrl(_urls);
            if (url.IsNotEmpty())
            {
                div.Data("url", url);
            }

            div.Next = _templates.WriteAll();

            return div;
        }
    }

    
}