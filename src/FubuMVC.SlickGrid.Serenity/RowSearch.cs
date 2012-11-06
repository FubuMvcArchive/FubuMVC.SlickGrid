using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Util;
using HtmlTags;
using OpenQA.Selenium;
using FubuCore.Reflection;
using FubuCore;
using Serenity;
using Serenity.Fixtures;
using System.Linq;

namespace FubuMVC.SlickGrid.Serenity
{
    public static class SlickGridSerenityWebDriverExtensions
    {
        public static GridAction<T> GridAction<T>(this IWebDriver driver, string id)
        {
            return new GridAction<T>(id, driver);
        } 
    }

    public class GridAction<T>
    {
        private readonly string _gridId;
        private readonly IWebDriver _driver;
        private readonly RowSearch<T> _search = new RowSearch<T>(); 

        public GridAction(string gridId, IWebDriver driver)
        {
            _gridId = gridId;
            _driver = driver;
        }

        public SearchExpression Where(Expression<Func<T, object>> expression)
        {
            return new SearchExpression(this, expression);
        }

        public class SearchExpression
        {
            private readonly GridAction<T> _parent;
            private readonly Expression<Func<T, object>> _expression;

            public SearchExpression(GridAction<T> parent, Expression<Func<T, object>> expression)
            {
                _parent = parent;
                _expression = expression;
            }

            public GridAction<T> Is(string searchTerm)
            {
                _parent._search.Add(_expression, searchTerm);
                return _parent;
            }             
        }

        public void ClickOnRow()
        {
            Retry.Twice(() => {
                var js = "$('#{0}').get(0).activateCell({1})".ToFormat(_gridId, _search.SearchTerm());
                _driver.InjectJavascript(js);

                var css = "#{0} .slick-cell.active".ToFormat(_gridId);
                _driver.FindElement(By.CssSelector(css)).Click();
            });
        }

        public string TextFor(Expression<Func<T, object>> expression)
        {
            return Formatter(expression).Text;
        }

        public IWebElement Formatter(Expression<Func<T, object>> expression)
        {
            var id = Guid.NewGuid().ToString();

            var js = "$('#{0}').get(0).markCell({1}, '{2}', '{3}')".ToFormat(_gridId, _search.SearchTerm(), expression.ToAccessor().Name, id);
            _driver.InjectJavascript(js);


            return _driver.FindElement(By.Id(id));
        }

        public void Activate(Expression<Func<T, object>> expression)
        {
            var js = "$('#{0}').get(0).activateCell({1}, '{2}')".ToFormat(_gridId, _search.SearchTerm(), expression.ToAccessor().Name);
            _driver.InjectJavascript(js);
        }

        public IWebElement Editor(Expression<Func<T, object>> expression)
        {
            return Retry.Twice(() => {
                var js = "$('#{0}').get(0).editCell({1}, '{2}')".ToFormat(_gridId, _search.SearchTerm(),
                                                                          expression.ToAccessor().Name);
                _driver.InjectJavascript(js);

                var css = "#{0} .slick-cell.active".ToFormat(_gridId);
                return _driver.FindElement(By.CssSelector(css)).FindElements(By.CssSelector("*")).FirstOrDefault();
            });
        }



    }

    public class RowSearch<T>
    {
        private readonly Cache<string, string> _props = new Cache<string, string>(); 

        public void Add(Expression<Func<T, object>> expression, string term)
        {
            _props[expression.ToAccessor().Name] = term;
        }

        public string SearchTerm()
        {
            var list = new List<string>();
            _props.Each((key, value) => {
                list.Add("{0}:'{1}'".ToFormat(key, value));
            });

            return "{" + list.Join(", ") + "}";
        }
    }
}