using System.Collections.Generic;
using System.Text;

namespace FubuMVC.SlickGrid
{
    public enum FieldType
    {
        column,
        dataOnly
    }

    public interface IGridColumn<T>
    {
        void WriteColumn(StringBuilder builder);
        void WriteField(T target, IDictionary<string, object> dictionary);

        FieldType FieldType { get; }
    }
}