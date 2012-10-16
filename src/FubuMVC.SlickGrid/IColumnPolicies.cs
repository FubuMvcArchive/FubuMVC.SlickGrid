using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using System.Linq;

namespace FubuMVC.SlickGrid
{
    public interface IColumnPolicies
    {
        SlickGridEditor EditorFor(Accessor accessor);
        SlickGridFormatter FormatterFor(Accessor accessor);
    }

    // TODO -- make this thing smarter later
    public class ColumnPolicies : IColumnPolicies
    {
        private readonly IList<IEditorSelection> _editorSelections = new List<IEditorSelection>();
        private readonly IList<IFormatterSelection> _formatterSelections = new List<IFormatterSelection>();
        private readonly IList<IColumnRule> _rules = new List<IColumnRule>(); 

        public SlickGridEditor EditorFor(Accessor accessor)
        {
            var selection = _editorSelections.FirstOrDefault(x => x.Matches(accessor));
            return selection == null ? SlickGridEditor.Text : selection.EditorFor(accessor);
        }

        public SlickGridFormatter FormatterFor(Accessor accessor)
        {
            var selection = _formatterSelections.FirstOrDefault(x => x.Matches(accessor));
            return selection == null ? null : selection.FormatterFor(accessor);
        }

        public IEnumerable<IColumnRule> RulesFor(Accessor accessor)
        {
            return _rules.Where(x => x.Matches(accessor));
        } 

        public void AddColumnRule<T>() where T : IColumnRule, new()
        {
            _rules.Add(new T());
        }

        public void AddFormatterPolicy<T>() where T : IFormatterSelection, new()
        {
            _formatterSelections.Add(new T());
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

            public FilterExpression FormatWith(SlickGridFormatter slickGridFormatter)
            {
                _parent._formatterSelections.Add(new FormatterSelection(_filter, slickGridFormatter));
                return this;
            }

            public FilterExpression Alter(Action<IGridColumn> alteration)
            {
                _parent._rules.Add(new LambdaColumnRule(_filter, alteration));
                return this;
            }

            public FilterExpression SetProperty(string property, object value)
            {
                return Alter(c => c.Properties[property] = value);
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

    public interface IFormatterSelection
    {
        bool Matches(Accessor accessor);
        SlickGridFormatter FormatterFor(Accessor accessor);
    }

    public class FormatterSelection : IFormatterSelection
    {
        private readonly Func<Accessor, bool> _filter;
        private readonly SlickGridFormatter _Formatter;

        public FormatterSelection(Func<Accessor, bool> filter, SlickGridFormatter Formatter)
        {
            _filter = filter;
            _Formatter = Formatter;
        }

        public bool Matches(Accessor accessor)
        {
            return _filter(accessor);
        }

        public SlickGridFormatter FormatterFor(Accessor accessor)
        {
            return _Formatter;
        }
    }

    public interface IColumnRule
    {
        bool Matches(Accessor accessor);
        void Alter(IGridColumn column);
    }

    public class LambdaColumnRule : IColumnRule
    {
        private readonly Func<Accessor, bool> _filter;
        private readonly Action<IGridColumn> _alteration;

        public LambdaColumnRule(Func<Accessor, bool> filter, Action<IGridColumn> alteration)
        {
            _filter = filter;
            _alteration = alteration;
        }

        public bool Matches(Accessor accessor)
        {
            return _filter(accessor);
        }

        public void Alter(IGridColumn column)
        {
            _alteration(column);
        }
    }
}