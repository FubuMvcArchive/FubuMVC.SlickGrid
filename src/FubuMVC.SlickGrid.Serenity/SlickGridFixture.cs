using System;
using System.Linq.Expressions;
using Serenity.Fixtures;
using StoryTeller;

namespace FubuMVC.SlickGrid.Serenity
{
    public abstract class SlickGridFixture<T> : ScreenFixture
    {
        private readonly string _gridId;
        private readonly Lazy<GridDriver> _driver; 

        protected SlickGridFixture(string gridId)
        {
            _gridId = gridId;
            _driver = new Lazy<GridDriver>(() => new GridDriver(Driver, gridId));
        }

        protected GridAction<T>.SearchExpression Where(Expression<Func<T, object>> expression)
        {
            return Driver.GridAction<T>(_gridId).Where(expression);
        }

        public IGrammar TheDisplayedColumnFieldsAre()
        {
            return VerifyStringList(() => _driver.Value.DisplayedColumnFields()).Ordered().Titled("The displayed fields should be")
                .Grammar();
        }

        public IGrammar TheFrozenColumnFieldsAre()
        {
            return VerifyStringList(() => _driver.Value.FrozenColumnFields()).Ordered().Titled("The frozen fields should be")
                .Grammar();
        }
    }
}