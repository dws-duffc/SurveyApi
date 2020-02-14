//-----------------------------------------------------------------------
// <copyright file=”ExpressionDecompiler.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides a means of reverse engineering an expression into a List of type Filter.
    /// Currently only works for NodeType ExpressionType.AndAlso with no Nested conditions.
    /// </summary>
    /// <typeparam name="TEntity">Used to get member name for Filter List.</typeparam>
    public static class ExpressionDecompiler<TEntity> where TEntity : class
    {
        /// <summary>
        /// Decompiles a lambda expression into a List of type Filter.
        /// </summary>
        /// <param name="expression">Lambda binary expression</param>
        /// <returns>List of type Filter.</returns>
        public static List<Filter> Decompile(Expression<Func<TEntity, bool>> expression)
        {
            var convertedExp = expression.Body as BinaryExpression;

            if (convertedExp == null)
            {
                throw new NotSupportedException("ExpressionDecompiler only supports BinaryExpressions.");
            }

            var filters = new List<Filter>();
            ParseExpression(convertedExp, ref filters);

            return filters;
        }

        /// <summary>
        /// Recursively parses Expression, depth-first, adding to Filter List at the end of each traversal.
        /// </summary>
        /// <param name="expression">Expression to be traversed.</param>
        /// <param name="filters">List to which filters are added when end of traversal is reached.</param>
        private static void ParseExpression(Expression expression, ref List<Filter> filters)
        {
            if (expression is BinaryExpression)
            {
                if (expression.NodeType != ExpressionType.AndAlso
                    && expression.NodeType != ExpressionType.Equal
                    && expression.NodeType != ExpressionType.MemberAccess)
                { throw new NotSupportedException("ExpressionDecompiler only supports && and == operators."); }

                var binExp = expression as BinaryExpression;
                var filter = new Filter();

                if (binExp.Left is MemberExpression)
                {
                    filter.PropertyName = ((MemberExpression)binExp.Left).Member.Name;
                }
                else if (binExp.Left is ConstantExpression)
                {
                    filter.PropertyName = Convert.ToString(Expression.Lambda(((ConstantExpression)binExp.Left)).Compile().DynamicInvoke());
                }

                if (binExp.Right is ConstantExpression)
                {
                    filter.Value = Expression.Lambda(((ConstantExpression)binExp.Right)).Compile().DynamicInvoke();
                }
                else if (binExp.Right is MemberExpression)
                {
                    filter.Value = Expression.Lambda(((MemberExpression)binExp.Right)).Compile().DynamicInvoke();
                }

                if (filter.PropertyName != null && filter.Value != null)
                {
                    string opChar = string.Empty;
                    filter.Operation = GetOperation(binExp.NodeType, ref opChar);
                    filter.OpChar = opChar;
                    filters.Add(filter);
                }

                ParseExpression(binExp.Left, ref filters);
                ParseExpression(binExp.Right, ref filters);
            }
        }

        /// <summary>
        /// Provides the Operation enum equivalent of the given ExpressionType.
        /// </summary>
        /// <param name="nodeType">ExpressionType from lambda.</param>
        /// <returns>Operation enum equivalent of give ExpressionType</returns>
        private static Operation GetOperation(ExpressionType nodeType, ref string opChar)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    opChar = "=";
                    return Operation.Equals;
                case ExpressionType.GreaterThan:
                    opChar = ">";
                    return Operation.GreaterThan;
                case ExpressionType.LessThan:
                    opChar = "<";
                    return Operation.LessThan;
                case ExpressionType.GreaterThanOrEqual:
                    opChar = ">=";
                    return Operation.GreaterThanOrEqual;
                default:
                    opChar = "<=";
                    return Operation.LessThanOrEqual;
            }
        }
    }
}
