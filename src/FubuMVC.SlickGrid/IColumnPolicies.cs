using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using System.Linq;

namespace FubuMVC.SlickGrid
{
    public interface IColumnPolicies
    {
        SlickGridEditor EditorFor(Accessor accessor);
    }

    // TODO -- make this thing smarter later
    public class ColumnPolicies : IColumnPolicies
    {
        private readonly IList<IEditorSelection> _editorSelections = new List<IEditorSelection>();

        public SlickGridEditor EditorFor(Accessor accessor)
        {
            var selection = _editorSelections.FirstOrDefault(x => x.Matches(accessor));
            return selection == null ? SlickGridEditor.Text : selection.EditorFor(accessor);
        }

        public void AddEditorPolicy<T>() where T : IEditorSelection, new()
        {
            _editorSelections.Add(new T());
        }

        public FilterExpression If(Func<Accessor, bool> filter)
        {
            return new FilterExpression(this, filter);
        }

        public class FilterExpression
        {
            private readonly ColumnPolicies _parent;
            private readonly Func<Accessor, bool> _filter;

            public FilterExpression(ColumnPolicies parent, Func<Accessor, bool> filter)
            {
                _parent = parent;
                _filter = filter;
            }

            public FilterExpression EditWith(SlickGridEditor slickGridEditor)
            {
                _parent._editorSelections.Add(new EditorSelection(_filter, slickGridEditor));
                return this;
            }
        }
    }

    public interface IEditorSelection
    {
        bool Matches(Accessor accessor);
        SlickGridEditor EditorFor(Accessor accessor);
    }

    public class EditorSelection : IEditorSelection
    {
        private readonly Func<Accessor, bool> _filter;
        private readonly SlickGridEditor _editor;

        public EditorSelection(Func<Accessor, bool> filter, SlickGridEditor editor)
        {
            _filter = filter;
            _editor = editor;
        }

        public bool Matches(Accessor accessor)
        {
            return _filter(accessor);
        }

        public SlickGridEditor EditorFor(Accessor accessor)
        {
            return _editor;
        }
    }
}