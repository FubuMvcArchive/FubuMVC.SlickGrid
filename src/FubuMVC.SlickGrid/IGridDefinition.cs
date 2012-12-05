using FubuMVC.Core.UI.Templates;
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

        void SelectFormattersAndEditors(IColumnPolicies editors);
        void WriteAnyTemplates(ITemplateWriter writer);
        bool UsesHtmlConventions { get; set; }
        bool AllColumnsAreEditable { get; set; }

        SlickGridFormatter DefaultFormatter { get; set; }
    }
}