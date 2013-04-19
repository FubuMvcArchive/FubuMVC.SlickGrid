using System;
using System.Collections.Generic;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Templates;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;

namespace FubuMVC.SlickGrid
{
    public interface IGridDefinition<T> : IGridDefinition
    {
        Projection<T> ToProjection(IFieldAccessService accessService);
    }

    public interface IGridDefinition
    {
        IEnumerable<IGridColumn> Columns();
        Type DetermineRunnerType();
        string ToColumnJson(IFieldAccessService accessService);
        string SelectDataSourceUrl(IUrlRegistry urls);

        void SelectFormattersAndEditors(IColumnPolicies editors);
        void WriteAnyTemplates(ITemplateWriter writer);
        bool UsesHtmlConventions { get; set; }
        bool AllColumnsAreEditable { get; set; }

        SlickGridFormatter DefaultFormatter { get; set; }

        bool IsPaged();
    }
}