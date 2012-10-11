using System.Text;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Templates;
using FubuMVC.Media.Projections;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class ColumnDefinitionTester
    {
        private Projection<ColumnDefTarget> theProjection;

        [SetUp]
        public void SetUp()
        {
            theProjection = new Projection<ColumnDefTarget>();
        }

        private string writeColumn(IGridColumn column)
        {
            var builder = new StringBuilder();
            column.WriteColumn(builder);

            return builder.ToString();
        }

        [Test]
        public void is_editable_is_false_by_default()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Editable().ShouldBeFalse();
        }

        [Test]
        public void can_set_the_editable()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Editable(true);
            column.Editable().ShouldBeTrue();
        }

        [Test]
        public void adds_an_accessor_projection_to_the_projection()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.ProjectBy(x => {
                x.Name().ShouldEqual("Name");
            });
        }

        [Test]
        public void sortable_by_default()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            writeColumn(column).ShouldContain("sortable: true");
        }

        [Test]
        public void override_title()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Title("else");

            writeColumn(column).ShouldContain("name: \"else\"");
        }

        [Test]
        public void override_editor()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Editor(SlickGridEditor.Text);

            writeColumn(column).ShouldContain("editor: " + SlickGridEditor.Text.Name);
        }

        [Test]
        public void override_editor_by_name()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Editor(SlickGridEditor.Text.Name);

            writeColumn(column).ShouldContain("editor: " + SlickGridEditor.Text.Name);
        }

        [Test]
        public void override_field()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Field("else");

            writeColumn(column).ShouldEqual("{name: \"Name\", field: \"else\", id: \"Name\", sortable: true}");
        }

        [Test]
        public void override_id()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Id("else");

            writeColumn(column).ShouldEqual("{name: \"Name\", field: \"Name\", id: \"else\", sortable: true}");  
        }

        [Test]
        public void overwrite_sortable()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection).Sortable(false);
            writeColumn(column).ShouldContain("sortable: false"); 
        }

        [Test]
        public void override_resizable()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection).Resizable(false);

            writeColumn(column).ShouldContain("resizable: false");
        }

        [Test]
        public void override_resizable_2()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection).Resizable(true);

            writeColumn(column).ShouldContain("resizable: true");
        }

        [Test]
        public void write_column_basic_with_defaults()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);

            writeColumn(column).ShouldEqual("{name: \"Name\", field: \"Name\", id: \"Name\", sortable: true}");           
        }

        [Test]
        public void write_column_for_widths()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Width(100, 80, 120);

            writeColumn(column).ShouldContain("width: 100, minWidth: 80, maxWidth: 120");
        }

        [Test]
        public void select_formatter_uses_the_default_formatter_if_one_exists()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);

            var grid = MockRepository.GenerateMock<IGridDefinition>();

            var defaultFormatter = new SlickGridFormatter("default");
            grid.Stub(x => x.DefaultFormatter).Return(defaultFormatter);

            column.SelectFormatterAndEditor(grid, new ColumnPolicies());

            column.Formatter().ShouldBeTheSameAs(defaultFormatter);
        }

        [Test]
        public void does_not_override_an_explicit_formatter_with_the_default()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, theProjection);
            column.Formatter(SlickGridFormatter.TypeFormatter);

            var grid = MockRepository.GenerateMock<IGridDefinition>();

            var defaultFormatter = new SlickGridFormatter("default");
            grid.Stub(x => x.DefaultFormatter).Return(defaultFormatter);

            column.SelectFormatterAndEditor(grid, new ColumnPolicies());

            column.Formatter().ShouldEqual(SlickGridFormatter.TypeFormatter);
        }
    }

    [TestFixture]
    public class when_writing_templates
    {
        private ColumnDefinition<ColumnDefTarget, string> theColumn;
        private ITemplateWriter theTemplates;

        [SetUp]
        public void SetUp()
        {
            theColumn = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name, new Projection<ColumnDefTarget>());
            theTemplates = MockRepository.GenerateMock<ITemplateWriter>();
        }

        [Test]
        public void do_nothing_if_not_using_any_kind_of_template()
        {
            theColumn.As<IGridColumn>().WriteTemplates(theTemplates);
            theTemplates.AssertWasNotCalled(x => x.AddElement(theColumn.Accessor, ElementConstants.Display));
            theTemplates.AssertWasNotCalled(x => x.AddElement(theColumn.Accessor, ElementConstants.Editor));
        }

        [Test]
        public void do_nothing_if_using_explicit_formatters_that_are_not_the_underscores()
        {
            theColumn.Editor(SlickGridEditor.Text);
            theColumn.Formatter(SlickGridFormatter.TypeFormatter);

            theColumn.As<IGridColumn>().WriteTemplates(theTemplates);
            theTemplates.AssertWasNotCalled(x => x.AddElement(theColumn.Accessor, ElementConstants.Display));
            theTemplates.AssertWasNotCalled(x => x.AddElement(theColumn.Accessor, ElementConstants.Editor));
        }

        [Test]
        public void write_the_display_template_if_using_underscore_formatter()
        {
            theColumn.Formatter(SlickGridFormatter.Underscore);

            theColumn.As<IGridColumn>().WriteTemplates(theTemplates);
            theTemplates.AssertWasCalled(x => x.AddElement(theColumn.Accessor, ElementConstants.Display));
        }

        [Test]
        public void write_the_editor_template_if_using_underscore_formatter()
        {
            theColumn.Editor(SlickGridEditor.Underscore);
            theColumn.As<IGridColumn>().WriteTemplates(theTemplates);
            theTemplates.AssertWasCalled(x => x.AddElement(theColumn.Accessor, ElementConstants.Editor));


        }
    }

    public class ColumnDefTarget
    {
        public string Name { get; set; }
        public bool IsCool { get; set; }
        public int Count { get; set; }
    }
}