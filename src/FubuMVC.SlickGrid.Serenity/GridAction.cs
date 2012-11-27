using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using OpenQA.Selenium;
using Serenity;
using Serenity.Fixtures;

namespace FubuMVC.SlickGrid.Serenity
{
    public class GridAction<T> : IGridAction<T>
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

            public IGridAction<T> Is(string searchTerm)
            {
                _parent._search.Add(_expression, searchTerm);
                return _parent;
            }             
        }

        public IWebElement Row()
        {
            return Retry.Twice(() =>
                               {
                                   var js = "return $('#{0}').get(0).findRowIndex({1})".ToFormat(_gridId, _search.SearchTerm());
                                   var index = _driver.InjectJavascript<long>(js);

                                   var elements = _driver.FindElement(By.Id(_gridId)).FindElements(By.CssSelector("div.slick-row")).ToArray();
                                   return elements[index];
                               });
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

        public bool IsSelected()
        {
            return Row().FindElements(By.CssSelector(".slick-cell.selected")).Any();
        }

        public string TextFor(Expression<Func<T, object>> expression)
        {
            return Formatter(expression).Text;
        }

        public IWebElement Formatter(Expression<Func<T, object>> expression)
        {
            var name = expression.ToAccessor().Name;

            return Formatter(name);
        }

        public IWebElement Formatter(string name)
        {
            return Retry.FiveTimes(() => {
                                             var id = Guid.NewGuid().ToString();
                                             var js = "$('#{0}').get(0).markCell({1}, '{2}', '{3}')".ToFormat(_gridId, _search.SearchTerm(), name, id);
                                             _driver.InjectJavascript(js);


                                             return _driver.FindElement(By.Id(id));
            });
        }

        public void Activate(Expression<Func<T, object>> expression)
        {
            var js = "$('#{0}').get(0).activateCell({1}, '{2}')".ToFormat(_gridId, _search.SearchTerm(), expression.ToAccessor().Name);
            _driver.InjectJavascript(js);
        }

        public IWebElement Editor(Expression<Func<T, object>> expression)
        {
            var name = expression.ToAccessor().Name;

            return Editor(name);
        }

        public IWebElement Editor(string name)
        {
            return Retry.FiveTimes(() =>
                                   {

                                       var js = "$('#{0}').get(0).editCell({1}, '{2}')".ToFormat(_gridId, _search.SearchTerm(),
                                                                                                 name);
                                       _driver.InjectJavascript(js);

                                       var css = "#{0} .slick-cell.active".ToFormat(_gridId);
                                       return _driver.FindElement(By.CssSelector(css)).FindElements(By.CssSelector("*")).FirstOrDefault();
                                   });
        }

        SearchExpression IGridAction<T>.And(Expression<Func<T, object>> expression)
        {
            return new SearchExpression(this, expression);
        }



    }
}