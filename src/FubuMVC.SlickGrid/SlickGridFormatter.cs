namespace FubuMVC.SlickGrid
{
    public class SlickGridFormatter
    {
        public static readonly SlickGridFormatter TypeFormatter = new SlickGridFormatter("Slick.Formatters.DotNetType");
        public static readonly SlickGridFormatter StringArray = new SlickGridFormatter("Slick.Formatters.StringArray");
        public static readonly SlickGridFormatter Underscore = new SlickGridFormatter("Slick.Formatters.Underscore");
        

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

        protected bool Equals(SlickGridFormatter other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SlickGridFormatter) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }
    }
}