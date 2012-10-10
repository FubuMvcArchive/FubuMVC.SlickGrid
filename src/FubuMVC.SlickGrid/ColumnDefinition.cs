using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Media.Projections;

namespace FubuMVC.SlickGrid
{
    public class SlickGridFormatter
    {
        public static readonly SlickGridFormatter TypeFormatter = new SlickGridFormatter("Slick.Formatters.DotNetType");
        public static readonly SlickGridFormatter StringArray = new SlickGridFormatter("Slick.Formatters.StringArray");
        

        private readonly string _name;

        public SlickGridFormatter(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    public class SlickGridEditor
    {
        public static readonly SlickGridEditor Text = new SlickGridEditor("Slick.Editors.Text");

        private readonly string _name;

        public SlickGridEditor(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    public class ColumnDefinition<T, TProp> : IGridColumn<T>
    {
        private readonly FieldType _fieldType;
        private readonly Accessor _accessor;
        private readonly Cache<string, object> _cache;
        private readonly AccessorProjection<T, TProp> _projection;

        public ColumnDefinition(FieldType fieldType, Expression<Func<T, TProp>> property, Projection<T> projection)
        {
            _fieldType = fieldType;
            _cache = new Cache<string, object>();

            _accessor = ReflectionHelper.GetAccessor(property);

            _projection = projection.Value(property);

            Title(_accessor.Name);
            Field(_accessor.Name);
            Id(_accessor.Name);

            Sortable(true);
        }

        public ColumnDefinition<T, TProp> ProjectWith<TProjector>() where TProjector : IValueProjector<TProp>, new()
        {
            _projection.ProjectWith<TProjector>();
            return this;
        }

        public ColumnDefinition<T, TProp> ProjectBy(Action<AccessorProjection<T, TProp>> configuration)
        {
            configuration(_projection);
            return this;
        } 

        void IGridColumn<T>.WriteColumn(StringBuilder builder)
        {
            builder.Append("{");

            _cache.Each((key, value) =>
            {
                builder.WriteJsonProp(key, value);
                builder.Append(", ");
            });
            

            builder.Remove(builder.Length - 2, 2);
            builder.Append("}");
        }

        public FieldType FieldType
        {
            get { return _fieldType; }
        }

        void IGridColumn<T>.Editor(string editor)
        {
            Editor(new SlickGridEditor(editor));
        }

        void IGridColumn<T>.Editor(SlickGridEditor editor)
        {
            Editor(editor);
        }

        public ColumnDefinition<T, TProp> Editor(string editor)
        {
            return Editor(new SlickGridEditor(editor));
        }

        public ColumnDefinition<T, TProp> Editor(SlickGridEditor editor)
        {
            _cache["editor"] = editor;
            return this;
        }

        /// <summary>
        ///   True by default
        /// </summary>
        /// <param name = "isSortable"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Sortable(bool isSortable)
        {
            _cache["sortable"] = isSortable;

            return this;
        }

        /// <summary>
        ///   By default, this is the name of the property
        /// </summary>
        /// <param name = "title"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Title(string title)
        {
            _cache["name"] = title;

            return this;
        }

        /// <summary>
        ///   By default, this is the name of the property
        /// </summary>
        /// <param name = "field"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Field(string field)
        {
            _cache["field"] = field;
            return this;
        }

        /// <summary>
        ///   By default, this is the name of the property
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        public ColumnDefinition<T, TProp> Id(string id)
        {
            _cache["id"] = id;
            return this;
        }

        public ColumnDefinition<T, TProp> Resizable(bool resizable)
        {
            _cache["resizable"] = resizable;
            return this;
        }

        public ColumnDefinition<T, TProp> Width(int width = 0, int minWidth = 0, int maxWidth = 0)
        {
            if (width > 0)
            {
                _cache["width"] = width;
            }

            if (minWidth > 0)
            {
                _cache["minWidth"] = minWidth;
            }

            if (maxWidth > 0)
            {
                _cache["maxWidth"] = maxWidth;
            }

            return this;
        }

        public ColumnDefinition<T, TProp> Property(string property, object value)
        {
            _cache[property] = value;
            return this;
        }

        public ColumnDefinition<T, TProp> Formatter(string formatter)
        {
            _cache["formatter"] = new SlickGridFormatter(formatter);
            return this;
        }

        public ColumnDefinition<T, TProp> Formatter(SlickGridFormatter formatter)
        {
            _cache["formatter"] = formatter;
            return this;
        }

        public override string ToString()
        {
            return string.Format("Accessor: {0}, FieldType: {1}", _accessor.PropertyNames.Join("."), _fieldType);
        }
    }
}