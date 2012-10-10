using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.SlickGrid
{
    public abstract class GridDefinition<T> : IGridDefinition<T>, IFubuRegistryExtension
    {
        private readonly IList<IGridColumn<T>> _columns = new List<IGridColumn<T>>();
        private Type _queryType;
        private Type _sourceType;

        protected GridDefinition()
        {
            Projection = new Projection<T>();
        }

        public Type SourceType
        {
            get { return _sourceType; }
        }

        public AddExpression Add
        {
            get { return new AddExpression(this); }
        }

        #region IFubuRegistryExtension Members

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Configure(graph => {
                Type runnerType = DetermineRunnerType();


                MethodInfo method = runnerType.GetMethod("Run");

                var call = new ActionCall(runnerType, method);
                var chain = new BehaviorChain();
                chain.AddToEnd(call);
                chain.Route = new RouteDefinition(DiagnosticConstants.UrlPrefix);
                chain.Route.Append("_data");
                chain.Route.Append(typeof (T).Name);

                chain.MakeAsymmetricJson();

                graph.AddChain(chain);
            });
        }

        #endregion

        #region IGridDefinition<T> Members

        string IGridDefinition.ToColumnJson()
        {
            var builder = new StringBuilder();
            builder.Append("[");

            List<IGridColumn<T>> columns = _columns.Where(x => x.FieldType == FieldType.column).ToList();
            for (int i = 0; i < columns.Count - 1; i++)
            {
                IGridColumn<T> column = columns[i];
                column.WriteColumn(builder);
                builder.Append(", ");
            }

            columns.Last().WriteColumn(builder);

            builder.Append("]");

            return builder.ToString();
        }

        string IGridDefinition.SelectDataSourceUrl(IUrlRegistry urls)
        {
            if (_sourceType == null) return null;

            if (_queryType != null)
            {
                return urls.UrlFor(_queryType);
            }

            Type runnerType = DetermineRunnerType();

            return urls.UrlFor(runnerType);
        }

        public Projection<T> Projection { get; private set; }

        #endregion

        /// <summary>
        /// Source type must implement either IGridDataSource<T> or IGridDataSource<T, TQuery>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        public void SourceIs<TSource>()
        {
            Type sourceType = typeof (TSource);
            var templateType = sourceType.FindInterfaceThatCloses(typeof (IGridDataSource<>));
            if (templateType != null)
            {
                if (templateType.GetGenericArguments().First() != typeof (T))
                {
                    throw new ArgumentOutOfRangeException("Wrong type as the argument to IGridDataSource<>");
                }

                _queryType = null;
                _sourceType = sourceType;

                return;
            }

            templateType = sourceType.FindInterfaceThatCloses(typeof (IGridDataSource<,>));
            if (templateType != null)
            {
                if (templateType.GetGenericArguments().First() != typeof (T))
                {
                    throw new ArgumentOutOfRangeException("Wrong type as the argument to IGridDataSource<>");
                }

                _queryType = templateType.GetGenericArguments().Last();
                _sourceType = sourceType;

                return;
            }

            throw new ArgumentOutOfRangeException("TSource must be either IGridDataSource<T> or IGridDataSource<TQuery>");
        }

        public ColumnDefinition<T, TProp> Column<TProp>(Expression<Func<T, TProp>> property)
        {
            var column = new ColumnDefinition<T, TProp>(FieldType.column, property, Projection);
            _columns.Add(column);

            return column;
        }

        public ColumnDefinition<T, TProp> Data<TProp>(Expression<Func<T, TProp>> property)
        {
            var column = new ColumnDefinition<T, TProp>(FieldType.dataOnly, property, Projection);
            _columns.Add(column);

            return column;
        }

        public Type DetermineRunnerType()
        {
            return _queryType == null
                       ? typeof (GridRunner<,,>).MakeGenericType(typeof (T), GetType(), _sourceType)
                       : typeof (GridRunner<,,,>).MakeGenericType(typeof (T), GetType(), _sourceType, _queryType);
        }

        #region Nested type: AddExpression

        public class AddExpression
        {
            private readonly GridDefinition<T> _parent;

            public AddExpression(GridDefinition<T> parent)
            {
                _parent = parent;
            }

            public static AddExpression operator +(AddExpression original, IGridColumn<T> column)
            {
                original._parent._columns.Add(column);

                return original;
            }
        }

        #endregion
    }
}