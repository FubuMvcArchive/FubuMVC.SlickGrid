using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Util;
using FubuCore.Reflection;
using FubuCore;

namespace FubuMVC.SlickGrid.Serenity
{
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