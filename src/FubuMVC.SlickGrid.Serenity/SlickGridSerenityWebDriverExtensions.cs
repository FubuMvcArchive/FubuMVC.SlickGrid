using OpenQA.Selenium;

namespace FubuMVC.SlickGrid.Serenity
{
    public static class SlickGridSerenityWebDriverExtensions
    {
        public static GridAction<T> GridAction<T>(this IWebDriver driver, string id)
        {
            return new GridAction<T>(id, driver);
        } 
    }
}