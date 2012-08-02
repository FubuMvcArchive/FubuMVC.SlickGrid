using System;
using System.Web;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

namespace SlickGridHarness
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            FubuApplication.For<SlickGridHarnessRegistry>().StructureMap(new Container()).Bootstrap();
        }
    }
}