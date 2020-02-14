//-----------------------------------------------------------------------
// <copyright file=”ExpressionBuilder.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    /* EXAMPLE USE:
     * List<Filter> filters = new List<Filter> { new Filter { Operation = Operation.Equals, PropertyName = "AgentId", Value = 1 } };
     * Expression<Func<TEntity, bool>> deleg = ExpressionBuilder.GetExpression<TEntity>(filters);
     * return GetAll().Where(deleg.Compile()).ToList(); */
     /// <summary>
     /// Used to create a lambda expression from a List of type Filter.
     /// </summary>
    public static class ExpressionBuilder
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains");
        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        public static Expression<Func<T, bool>> GetExpression<T>(IList<Filter> filters)
        {
            if (filters.Count == 0)
            { return null; }

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
            { exp = GetExpression(param, filters[0]); }
            else if (filters.Count == 2)
            { exp = GetExpression(param, filters[0], filters[1]); }
            else
            {
                while (filters.Count > 0)
                {
                    Filter f1 = filters[0];
                    Filter f2 = filters[1];

                    if (exp == null)
                    { exp = GetExpression(param, filters[0], filters[1]); }
                    else
                    { exp = Expression.AndAlso(exp, GetExpression(param, filters[0], filters[1])); }

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = Expression.AndAlso(exp, GetExpression(param, filters[0]));
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp ?? throw new InvalidOperationException(), param);
        }

        private static Expression GetExpression(ParameterExpression param, Filter filter)
        {
            MemberExpression member = Expression.Property(param, filter.PropertyName);
            ConstantExpression constant = Expression.Constant(filter.Value);

            switch (filter.Operation)
            {
                case Operation.Equals:
                    return Expression.Equal(member, constant);

                case Operation.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case Operation.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case Operation.LessThan:
                    return Expression.LessThan(member, constant);

                case Operation.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);

                case Operation.Contains:
                    return Expression.Call(member, ContainsMethod, constant);

                case Operation.StartsWith:
                    return Expression.Call(member, StartsWithMethod, constant);

                case Operation.EndsWith:
                    return Expression.Call(member, EndsWithMethod, constant);
            }

            return null;
        }

        private static BinaryExpression GetExpression (ParameterExpression param, Filter filter1, Filter filter2)
        {
            Expression bin1 = GetExpression(param, filter1);
            Expression bin2 = GetExpression(param, filter2);

            return Expression.AndAlso(bin1, bin2);
        }
    }
}