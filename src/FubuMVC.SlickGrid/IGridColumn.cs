using System.Text;
using FubuCore.Reflection;

namespace FubuMVC.SlickGrid
{
    public interface IGridColumn
    {
        SlickGridEditor Editor { get; set; }

        bool IsEditable { get; }

        Accessor Accessor { get; }
        void WriteColumn(StringBuilder builder);
    }
}