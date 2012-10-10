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
}