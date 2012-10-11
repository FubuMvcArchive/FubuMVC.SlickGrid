using System.Text;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Templates;

namespace FubuMVC.SlickGrid
{
    public interface IGridColumn
    {
        SlickGridEditor Editor { get; set; }

        bool IsEditable { get; }

        Accessor Accessor { get; }
        void WriteColumn(StringBuilder builder);

        void WriteTemplates(ITemplateWriter writer);
        void SelectFormatterAndEditor(IGridDefinition grid, IColumnPolicies editors);
    }
}