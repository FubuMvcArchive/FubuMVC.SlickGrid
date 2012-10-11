namespace FubuMVC.SlickGrid
{
    public class SlickGridEditor
    {
        public static readonly SlickGridEditor Text = new SlickGridEditor("Slick.Editors.Text");
        public static readonly SlickGridEditor Underscore = new SlickGridEditor("Slick.Editors.UnderscoreTemplate");

        private readonly string _name;

        public SlickGridEditor(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        protected bool Equals(SlickGridEditor other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SlickGridEditor) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public string Name
        {
            get { return _name; }
        }


    }
}