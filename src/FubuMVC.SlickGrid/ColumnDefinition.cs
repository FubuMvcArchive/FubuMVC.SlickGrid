using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Templates;
using FubuMVC.Media.Projections;

namespace FubuMVC.SlickGrid
{
    public class ColumnDefinition<T, TProp> : IGridColumn
    {
        private const string EditorField = "editor";
        public const string FormatterField = "formatter";
        private readonly Cache<string, object> _cache;
        private readonly AccessorProjection<T, TProp> _projection;
        private bool _isEditable;

        public ColumnDefinition(Expression<Func<T, TProp>> property, Projection<T> projection)
        {
            _cache = new Cache<string, object>();

            Accessor = ReflectionHelper.GetAccessor(property);

            _projection = projection.Value(property);

            Title(Accessor.Name);
            Field(Accessor.Name);
            Id(Accessor.Name);

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

        public Accessor Accessor { get; private set; }

        void IGridColumn.WriteColumn(StringBuilder builder)
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

        void IGridColumn.WriteTemplates(ITemplateWriter writer)
        {
            if (Editor() == SlickGridEditor.Underscore)
            {
                writer.AddElement(Accessor, ElementConstants.Editor);
            }

            if (Formatter() == SlickGridFormatter.Underscore)
            {
                writer.AddElement(Accessor, ElementConstants.Display);
            }
        }

        public void SelectFormatterAndEditor(IGridDefinition grid, IColumnPolicies editors)
        {
            if (Editor() == null && grid.AllColumnsAreEditable)
            {
                if (grid.UsesHtmlConventions)
                {
                    Editor(SlickGridEditor.Underscore);
                    Property("editorSubject", TemplateWriter.SubjectFor(Accessor, ElementConstants.Editor));
                }
                else
                {
                    Editor(editors.EditorFor(Accessor));
                }
            }

            if (Formatter() == null && grid.UsesHtmlConventions)
            {
                Property("displaySubject", TemplateWriter.SubjectFor(Accessor, ElementConstants.Display));
                Formatter(SlickGridFormatter.Underscore);
            }
        }

        public ColumnDefinition<T, TProp> Editable(bool isEditable)
        {
            _isEditable = isEditable;
            return this;
        }

        public bool Editable()
        {
            return _isEditable || Editor() != null;
        }

        SlickGridEditor IGridColumn.Editor
        {
            get { return Editor(); }
            set { Editor(value); }
        }

        bool IGridColumn.IsEditable
        {
            get { return _isEditable; }
        }

        public ColumnDefinition<T, TProp> Editor(string editor)
        {
            return Editor(new SlickGridEditor(editor));
        }

        public ColumnDefinition<T, TProp> Editor(SlickGridEditor editor)
        {
            _cache[EditorField] = editor;
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

        public object Property(string property)
        {
            return _cache.Has(property) ? _cache[property] : null;
        }

        public ColumnDefinition<T, TProp> Formatter(string formatter)
        {
            _cache[FormatterField] = new SlickGridFormatter(formatter);
            return this;
        }

        public ColumnDefinition<T, TProp> Formatter(SlickGridFormatter formatter)
        {
            _cache[FormatterField] = formatter;
            return this;
        }

        public override string ToString()
        {
            return string.Format("Accessor: {0}", Accessor.PropertyNames.Join("."));
        }

        public SlickGridEditor Editor()
        {
            return _cache.Has(EditorField) ? _cache[EditorField] as SlickGridEditor : null;
        }

        public SlickGridFormatter Formatter()
        {
            return _cache.Has(FormatterField) ? _cache[FormatterField] as SlickGridFormatter : null;
        }
    }
}