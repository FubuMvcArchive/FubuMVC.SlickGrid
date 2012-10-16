using FubuCore.Reflection;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class GridDefinition_selecting_editors_specs
    {
        private TestGrid theGrid;
        private StubColumnPolicies theEditors = new StubColumnPolicies();

        [SetUp]
        public void SetUp()
        {
            theGrid = new TestGrid();
        }

        [Test]
        public void select_editor_does_nothing_to_a_column_that_is_not_editable_when_the_grid_is_not_editable()
        {
            theGrid.AllColumnsAreEditable = false;
            var column = theGrid.Column(x => x.Name).Editable(false);

            theGrid.As<IGridDefinition>().SelectFormattersAndEditors(theEditors);

            column.Editable().ShouldBeFalse();
            column.Editor().ShouldBeNull();
        }

        [Test]
        public void no_html_template_usage_all_fields_editable_no_explicit_selection_should_use_defaults()
        {
            theGrid.AllColumnsAreEditable = true;
            var column = theGrid.Column(x => x.Name);

            theGrid.As<IGridDefinition>().SelectFormattersAndEditors(theEditors);

            column.Editable().ShouldBeTrue();
            column.Editor().ShouldEqual(theEditors.EditorFor(column.Accessor));
        }

        [Test]
        public void all_fields_editable_does_not_override_an_explicit_choice()
        {
            theGrid.AllColumnsAreEditable = true;
            var column = theGrid.Column(x => x.Name).Editor(SlickGridEditor.Text);

            theGrid.As<IGridDefinition>().SelectFormattersAndEditors(theEditors);

            column.Editable().ShouldBeTrue();
            column.Editor().ShouldEqual(SlickGridEditor.Text);
        }

        [Test]
        public void all_fields_editable_with_html_conventions_turned_on_does_not_override_explicit()
        {
            theGrid.AllColumnsAreEditable = true;
            theGrid.UsesHtmlConventions = true;

            var column = theGrid.Column(x => x.Name).Editor(SlickGridEditor.Text);

            theGrid.As<IGridDefinition>().SelectFormattersAndEditors(theEditors);

            column.Editable().ShouldBeTrue();
            column.Editor().ShouldEqual(SlickGridEditor.Text);
        }

        [Test]
        public void all_fields_editable_with_html_conventions_sets_default_to_underscore()
        {
            theGrid.AllColumnsAreEditable = true;
            theGrid.UsesHtmlConventions = true;

            var column = theGrid.Column(x => x.Name);

            theGrid.As<IGridDefinition>().SelectFormattersAndEditors(theEditors);

            column.Editable().ShouldBeTrue();
            column.Editor().ShouldEqual(SlickGridEditor.Underscore);

            // This is a big, big problem!!!!  Not unique!!!  Needs to be better namespaced on the 
            // html conventions side of things!!!!
            column.Property("editorSubject").ShouldEqual("editor-GridDefTarget-Name");
        }

        [Test]
        public void uses_underscore_if_html_convention_usage_for_the_formatter_if_none_is_specified()
        {
            theGrid.UsesHtmlConventions = true;

            var column = theGrid.Column(x => x.Name);

            theGrid.As<IGridDefinition>().SelectFormattersAndEditors(theEditors);

            column.Formatter().ShouldEqual(SlickGridFormatter.Underscore);

            // This is a big, big problem!!!!  Not unique!!!  Needs to be better namespaced on the 
            // html conventions side of things!!!!
            column.Property("displaySubject").ShouldEqual("display-GridDefTarget-Name");
        }

        [Test]
        public void should_not_override_the_formatter_when_html_conventions_are_used_if_the_formatter_is_explicitly_chosen()
        {
            theGrid.UsesHtmlConventions = true;

            var column = theGrid.Column(x => x.Name).Formatter(SlickGridFormatter.StringArray);

            theGrid.As<IGridDefinition>().SelectFormattersAndEditors(theEditors);

            column.Formatter().ShouldEqual(SlickGridFormatter.StringArray);
        }

        public class TestGrid : GridDefinition<GridDefTarget>{}

        public class GridDefTarget
        {
            public string Name { get; set; }
            public bool IsCool { get; set; }
            public int Count { get; set; }
        }
    }

    public class StubColumnPolicies : IColumnPolicies
    {
        public SlickGridEditor EditorFor(Accessor accessor)
        {
            return new SlickGridEditor(accessor.Name);
        }

        public SlickGridFormatter FormatterFor(Accessor accessor)
        {
            return null;
        }
    }


}