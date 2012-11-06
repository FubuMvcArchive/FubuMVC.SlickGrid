using System;
using System.Linq.Expressions;
using Serenity.Fixtures;

namespace FubuMVC.SlickGrid.Serenity
{
    public abstract class SlickGridFixture<T> : ScreenFixture
    {
        private readonly string _gridId;

        protected SlickGridFixture(string gridId)
        {
            _gridId = gridId;
        }

        protected GridAction<T>.SearchExpression Where(Expression<Func<T, object>> expression)
        {
            return Driver.GridAction<T>(_gridId).Where(expression);
        } 
    }
}