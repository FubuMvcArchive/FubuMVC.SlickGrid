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

        FieldType FieldType { get; }
    }
}