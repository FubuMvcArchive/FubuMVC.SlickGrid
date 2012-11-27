using System;
using System.Linq.Expressions;
using OpenQA.Selenium;

namespace FubuMVC.SlickGrid.Serenity
{
    public interface IGridAction<T>
    {
        IWebElement Row();
        void ClickOnRow();

        bool IsSelected();

        string TextFor(Expression<Func<T, object>> expression);
        IWebElement Formatter(Expression<Func<T, object>> expression);
        void Activate(Expression<Func<T, object>> expression);
        IWebElement Editor(Expression<Func<T, object>> expression);
        GridAction<T>.SearchExpression And(Expression<Func<T, object>> expression);

        IWebElement Formatter(string name);
        IWebElement Editor(string name);
    }
}