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
        /// <param name="parameter">The <see cref="ParameterExpression"/> which will be replaced.</param>
        /// <param name="newExpression">The replacing <see cref="Expression"/>.</param>
        /// <returns>
        /// Returns a new <see cref="Expression"/> using the new <see cref="ParameterExpression"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// When the <see cref="Expression"/> is not one of the compatible types (
        /// <see cref="MemberExpression"/>, <see cref="MethodCallExpression"/>,
        /// <see cref="ParameterExpression"/>, <see cref="LambdaExpression"/>,
        /// <see cref="BinaryExpression"/>, <see cref="ConstantExpression"/>,
        /// <see cref="NewExpression"/>, <see cref="UnaryExpression"/>).
        /// </exception>
        public static Expression ReplaceParameter(this Expression expression, ParameterExpression parameter, Expression newExpression)
        {
            var expressionList = GetExpressionPath(expression);
            Expression expressionPart = null;

            // Build expression list with the new parameter expression.
            foreach (var subexpression in expressionList)
            {
                if (subexpression is ParameterExpression parameterExpr)
                {
                    // Replace parameter for new expression.
                    expressionPart = (parameterExpr == parameter ? newExpression : parameterExpr);
                }
                else if (subexpression is ConstantExpression constantExpression)
                {
                    expressionPart = constantExpression;
                }
                else if (subexpression is MemberExpression memberExpression)
                {
                    expressionPart = Expression.MakeMemberAccess(expressionPart, memberExpression.Member);
                }
                else if (subexpression is MethodCallExpression methodExpression)
                {
                    var arguments = methodExpression.Arguments.Select(x =>
                        ReplaceParameter(x, parameter, newExpression)
                    );
                    expressionPart = Expression.Call(expressionPart, methodExpression.Method, arguments);
                }
                else if (subexpression is BinaryExpression binaryExpression)
                {
                    expressionPart = Expression.MakeBinary(binaryExpression.NodeType,
                        ReplaceParameter(binaryExpression.Left, parameter, newExpression),
                        ReplaceParameter(binaryExpression.Right, parameter, newExpression));
                }
                else if (subexpression is NewExpression newExpr)
                {
                    expressionPart = Expression.New(newExpr.Constructor,
                        newExpr.Arguments.Select(x=> ReplaceParameter(x, parameter, newExpression)));
                }
                else if (subexpression is UnaryExpression unaryExpression)
                {
                    expressionPart = Expression.MakeUnary(
                        unaryExpression.NodeType, expressionPart, unaryExpression.Type);
                }
                else if (subexpression is LambdaExpression lambdaExpr)
                {
                    var newExpressions =
                        lambdaExpr.Parameters
                            .Select(x => x == parameter ? (ParameterExpression)newExpression : x);

                    // NOTE: It will perform exception if "newExpression" cannot cast to "ParameterExpression".
#if NET35
                    expressionPart = Expression.Lambda(expressionPart, newExpressions.ToArray());
#else
                    expressionPart = Expression.Lambda(expressionPart, lambdaExpr.CanReduce, newExpressions);
#endif
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
                else if (expressionPart is UnaryExpression unaryExpression)
                {
                    expressionList.Insert(0, unaryExpression);
                    expressionPart = unaryExpression.Operand;
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
