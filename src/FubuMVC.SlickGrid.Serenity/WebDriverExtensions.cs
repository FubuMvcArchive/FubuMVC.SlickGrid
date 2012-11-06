using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using OpenQA.Selenium;

namespace FubuMVC.SlickGrid.Serenity
{
    // TODO -- These go straight into Serenity
    public static class WebDriverExtensions
    {
        public static IWebElement InputFor<T>(this ISearchContext context, Expression<Func<T, object>> property)
        {
            return context.FindElement(By.Name(property.ToAccessor().Name));
        }

        public static IWebElement LabelFor<T>(this ISearchContext context, Expression<Func<T, object>> property)
        {
            return context.FindElement(By.CssSelector("label[for='{0}']".ToFormat(property.ToAccessor().Name)));
        }

        public static string Data(this IWebElement element, string attribute)
        {
            return element.GetAttribute("data-{0}".ToFormat(attribute));
        }

        public static IWebElement Parent(this IWebElement element)
        {
            return element.FindElement(By.XPath(".."));
        }

        public static IEnumerable<string> GetClasses(this IWebElement element)
        {
            return element
                .GetAttribute("class")
                .Split(' ');
        }

        public static bool HasClass(this IWebElement element, string className)
        {
            return element
                .GetClasses()
                .Contains(className);
        }

        public static string Value(this IWebElement element)
        {
            return element.GetAttribute("value");
        }

        public static string Id(this IWebElement element)
        {
            return element.GetAttribute("id");
        }

        public static string FindClasses(this IWebElement element, params string[] classes)
        {
            return classes.Where(c => element.HasClass(c)).Join(" ");
        }
    }
}