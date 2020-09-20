using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Defines the extension methods to the <see cref="System.Linq.Expressions"/> namespace.
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Creates a new <see cref="Expression"/> replacing the current <see cref="ParameterExpression"/> for a new one.
        /// </summary>
        /// <param name="expression">Original <see cref="Expression"/>.</param>
        /// <param name="newParameterExpression">The replacing <see cref="ParameterExpression"/>.</param>
        /// <returns>
        /// Returns a new <see cref="Expression"/> using the new <see cref="ParameterExpression"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// When the <see cref="Expression"/> is not one of the compatible types (
        /// <see cref="MemberExpression"/>, <see cref="MethodCallExpression"/>,
        /// <see cref="ParameterExpression"/>, <see cref="LambdaExpression"/>,
        /// <see cref="BinaryExpression"/>, <see cref="ConstantExpression"/>).
        /// </exception>
        public static Expression ReplaceParameter(this Expression expression, ParameterExpression newParameterExpression)
        {
            var expressionList = GetExpressionPath(expression);
            Expression expressionPart = null;

            // Build expression list with the new parameter expression.
            foreach (var subexpression in expressionList)
            {
                if (subexpression is ParameterExpression)
                {
                    expressionPart = newParameterExpression;
                }
                else if (subexpression is ConstantExpression constantExpr)
                {
                    expressionPart = constantExpr;
                }
                else if (subexpression is MemberExpression memberExpr)
                {
                    expressionPart = Expression.MakeMemberAccess(expressionPart, memberExpr.Member);
                }
                else if (subexpression is MethodCallExpression methodExpr)
                {
                    var arguments = methodExpr.Arguments.Select(x =>
                        x.ReplaceParameter(newParameterExpression)
                    );
                    expressionPart = Expression.Call(expressionPart, methodExpr.Method, arguments);
                }
                else if (subexpression is BinaryExpression binaryExpr)
                {
                    expressionPart = Expression.MakeBinary(binaryExpr.NodeType,
                        binaryExpr.Left.ReplaceParameter(newParameterExpression),
                        binaryExpr.Right.ReplaceParameter(newParameterExpression));
                }
                else if (subexpression is LambdaExpression lambdaExpr)
                {
                    expressionPart = Expression.Lambda(expressionPart, newParameterExpression);
                }
                else
                {
                    throw new NotSupportedException($"NodeType: {subexpression.NodeType} ({subexpression})");
                }
            }
            return expressionPart;
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> with the <see cref="Expression"/> parts of the <paramref name="expression"/> parameter.
        /// </summary>
        /// <param name="expression">Source <see cref="Expression"/></param>
        private static IEnumerable<Expression> GetExpressionPath(Expression expression)
        {
            var expressionList = new List<Expression>();
            var expressionPart = expression;

            do
            {
                if (expressionPart is ParameterExpression parameterExpression)
                {
                    expressionList.Insert(0, parameterExpression);
                    expressionPart = null;
                }
                else if (expressionPart is MemberExpression memberExpression)
                {
                    expressionList.Insert(0, memberExpression);
                    expressionPart = memberExpression.Expression;
                }
                else if (expressionPart is MethodCallExpression methodCallExpression)
                {
                    expressionList.Insert(0, methodCallExpression);
                    expressionPart = methodCallExpression.Object;
                }
                else if (expressionPart is LambdaExpression lambdaExpression)
                {
                    expressionList.Insert(0, lambdaExpression);
                    expressionPart = lambdaExpression.Body;
                }
                else
                {
                    expressionList.Insert(0, expressionPart);
                    expressionPart = null;
                }
            } while (expressionPart != null);

            return expressionList;
        }

        internal static void UpdateEndIndex(this ExpressionNode node)
        {
            var maxEndIndex = node.Nodes?.Max(x => x.EndIndex) ?? node.EndIndex;
            node.EndIndex = maxEndIndex;
        }

    }
}
