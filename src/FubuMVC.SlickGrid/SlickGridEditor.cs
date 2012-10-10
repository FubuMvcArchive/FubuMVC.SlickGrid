namespace FubuMVC.SlickGrid
{
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
}