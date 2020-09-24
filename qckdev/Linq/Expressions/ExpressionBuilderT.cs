using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using qckdev.Reflection;
using System.ComponentModel;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Provides an <see cref="Expression"/> generator from <see cref="ExpressionTree"/> items.
    /// </summary>
    /// <typeparam name="T">The object type defined in the <see cref="ExpressionTree"/>.</typeparam>
    /// <typeparam name="TResult">The <see cref="Type"/> returned for the <see cref="Expression"/>.</typeparam>
    public sealed class ExpressionBuilder<T, TResult>
    {

        private static MethodInfo GetMethod(Type type, string name, Type[] types)
#if PORTABLE
            => type.GetRuntimeMethod(name, types);
#else
            => type.GetMethod(name, types);
#endif

        private static readonly MethodInfo inMethod =
            GetMethod(typeof(qckdev.Extensions), nameof(qckdev.Extensions.In),
                new Type[] { typeof(string), typeof(Type[]) });
        private static readonly MethodInfo stringInMethod =
            GetMethod(typeof(qckdev.Extensions), nameof(qckdev.Extensions.In),
                new Type[] { typeof(string), typeof(bool), typeof(Type[]) });
        private static readonly MethodInfo stringEqualsMethod =
            GetMethod(typeof(System.String), nameof(System.String.Equals),
                new Type[] { typeof(string), typeof(string), typeof(StringComparison) });
        private static readonly MethodInfo stringLikeMethod =
            GetMethod(typeof(qckdev.Helper), nameof(qckdev.Helper.Like),
                new Type[] { typeof(string), typeof(string), typeof(StringLikeOptions) });


        private readonly Dictionary<ExpressionNodeType, Func<ExpressionNode, Expression>> expressionAction; // Templates.


        #region ctor

        /// <summary>
        /// Creates a new instance of <see cref="ExpressionBuilder{T, TResult}"/>.
        /// </summary>
        public ExpressionBuilder()
        {
            this.ItemType = typeof(T);
            this.Properties = ItemType.GetCachedProperties();
            this.ParameterExpression = Expression.Parameter(this.ItemType, "item");

            expressionAction = new Dictionary<ExpressionNodeType, Func<ExpressionNode, Expression>>()
            {
                { ExpressionNodeType.RelationalOperator, BuildRelationalExpression },
                { ExpressionNodeType.ArithmeticOperator, BuildArithmeticExpression },
                { ExpressionNodeType.LogicalOperator, BuildLogicalOperator },
                { ExpressionNodeType.PropertyType, BuildValueExpression },
                { ExpressionNodeType.StringType, BuildValueExpression },
                { ExpressionNodeType.DateType, BuildValueExpression },
                { ExpressionNodeType.UnknownType, BuildValueExpression },
                { ExpressionNodeType.ListType, BuildArrayExpression },
            };
        }

        #endregion


        #region properties

        /// <summary>
        /// Gets the object type defined in the <see cref="ExpressionBuilder{T, TResult}"/>.
        /// </summary>
        public Type ItemType { get; }

        /// <summary>
        /// Returns the <see cref="System.Linq.Expressions.ParameterExpression"/> used for Lambda expressions.
        /// </summary>
        public ParameterExpression ParameterExpression { get; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> with cached <see cref="PropertyInfo"/> for faster access.
        /// </summary>
        Dictionary<string, PropertyInfo> Properties { get; }

        #endregion


        #region methods

        /// <summary>
        /// Converts a <see cref="ExpressionNode"/> to <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionTree"/> to be converted.</param>
        /// <returns>The <see cref="Expression"/> result.</returns>
        /// <exception cref="KeyNotFoundException">When the <see cref="ExpressionNodeType"/> is not implemented.</exception>
        public Expression BuildExpression(ExpressionTree expression)
        {
            Expression rdo;

            rdo = BuildExpression(expression.Root);
            rdo = Expression.Convert(rdo, typeof(TResult));
            return rdo;
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> that represents accessing a property given the name of the property.
        /// </summary>
        /// <param name="propertyName">The name of a property.</param>
        /// <returns>
        /// A <see cref="MemberExpression"/> that has the <see cref="MemberExpression.Member"/>
        /// property set to the <see cref="PropertyInfo"/> that represents the property
        /// denoted by <paramref name="propertyName"/>.
        /// </returns>
        public MemberExpression BuildProperty(string propertyName)
        {
            return Expression.Property(this.ParameterExpression, propertyName);
        }

        /// <summary>
        /// Creates an <see cref="Expression"/> where the delegate type is known at compile time.
        /// </summary>
        /// <param name="expression"></param>
        public Expression<Func<T, TResult>> Lambda(Expression expression)
        {
            return Expression.Lambda<Func<T, TResult>>(expression, this.ParameterExpression);
        }

        /// <summary>
        /// Converts a <see cref="ExpressionNode"/> to <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionTree"/> to be converted.</param>
        /// <returns>The <see cref="Expression"/> lambda expression.</returns>
        /// <exception cref="KeyNotFoundException">When the <see cref="ExpressionNodeType"/> is not implemented.</exception>
        public static Expression<Func<T, TResult>> Create(ExpressionTree expression)
        {
            var eb = new ExpressionBuilder<T, TResult>();

            return eb.Lambda(eb.BuildExpression(expression));
        }

        /// <summary>
        /// Converts a <see cref="string"/> pattern to <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="string"/> pattern.</param>
        /// <returns>The <see cref="Expression"/> lambda expression.</returns>
        /// <exception cref="KeyNotFoundException">When the <see cref="ExpressionNodeType"/> is not implemented.</exception>
        public static Expression<Func<T, TResult>> Create(string expression)
        {
            var eb = new ExpressionBuilder<T, TResult>();
            var root = ExpressionStringBuilder.BuildTree(expression).Root;

            return (root == null ? null : eb.Lambda(eb.BuildExpression(root)));
        }

        #endregion

        #region expression methods

        /// <summary>
        /// Converts a <see cref="ExpressionNode"/> to <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionNode"/> to be converted.</param>
        /// <returns>The <see cref="Expression"/> result.</returns>
        /// <exception cref="KeyNotFoundException">When the <see cref="ExpressionNodeType"/> is not implemented.</exception>
        private Expression BuildExpression(ExpressionNode expression)
        {
            return expressionAction[expression.Type].Invoke(expression);
        }

        /// <summary>
        /// Converts from <see cref="ExpressionNode"/> to <see cref="Expression"/> 
        /// when the <see cref="ExpressionOperatorType"/> is a relational operator (==, !=, =, LIKE, &gt;, &lt;, &gt;=, &lt;=, ...).
        /// </summary>
        private Expression BuildRelationalExpression(ExpressionNode expression)
        {
            Expression rdo;

            if (expression.Type == ExpressionNodeType.RelationalOperator)
            {
                if (expression.Nodes.Count == 2)
                {
                    var val1 = BuildExpression(expression.Nodes[0]);
                    var val2 = BuildExpression(expression.Nodes[1]);

                    rdo = RelationalExpression(val1, expression.Operator, val2);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(expression.ToString());
                }
            }
            else
            {
                throw new ArgumentException(expression.Type.ToString());
            }
            return rdo;
        }

        /// <summary>
        /// Converts from <see cref="ExpressionNode"/> to <see cref="Expression"/> 
        /// when the <see cref="ExpressionOperatorType"/> is an arithmetic operator (+, -, *, /, ^, %, ...).
        /// </summary>
        private Expression BuildArithmeticExpression(ExpressionNode expression)
        {
            Expression rdo;

            if (expression.Type == ExpressionNodeType.ArithmeticOperator)
            {
                if (expression.Nodes.Count == 2)
                {
                    var val1 = BuildExpression(expression.Nodes[0]);
                    var val2 = BuildExpression(expression.Nodes[1]);

                    rdo = ArithmeticExpression(val1, expression.Operator, val2);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(expression.ToString());
                }
            }
            else
            {
                throw new ArgumentException(expression.Type.ToString());
            }
            return rdo;
        }

        /// <summary>
        /// Converts from <see cref="ExpressionNode"/> to <see cref="Expression"/> 
        /// when the <see cref="ExpressionOperatorType"/> is a logical operator (AND, OR, NOT, ...).
        /// </summary>
        private Expression BuildLogicalOperator(ExpressionNode expression)
        {
            Expression rdo;

            if (expression.Type == ExpressionNodeType.LogicalOperator)
            {
                if (expression.Operator == ExpressionOperatorType.Not)
                {
                    if (expression.Nodes.Count == 1)
                    {
                        var child = BuildRelationalExpression(expression.Nodes.First());

                        rdo = Expression.Not(child);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Operator '{expression.Operator}' cannot have more of one expression node.");
                    }
                }
                else if (expression.Nodes.Count > 0)
                {
                    var childs = expression.Nodes.Select(BuildExpression);

                    switch (expression.Operator)
                    {
                        case ExpressionOperatorType.And:
                            rdo = ExpressionHelper.Concatenate(Expression.AndAlso, childs);
                            break;
                        case ExpressionOperatorType.Or:
                            rdo = ExpressionHelper.Concatenate(Expression.OrElse, childs);
                            break;
                        default:
                            throw new NotSupportedException(expression.Operator.ToString());
                    }
                }
                else
                {
                    throw new ArgumentNullException(expression.ToString());
                }
            }
            else
            {
                throw new ArgumentException(expression.Type.ToString());
            }
            return rdo;
        }

        /// <summary>
        /// Converts from <see cref="ExpressionNode"/> to <see cref="Expression"/> 
        /// when the <see cref="ExpressionOperatorType"/> is a value expression.
        /// </summary>
        private Expression BuildValueExpression(ExpressionNode expression)
        {
            return BuildValueExpression(expression, expression.Type);
        }

        /// <summary>
        /// Converts from <see cref="ExpressionNode"/> to <see cref="Expression"/> 
        /// when the <see cref="ExpressionOperatorType"/> is a value expression.
        /// </summary>
        private Expression BuildValueExpression(ExpressionNode expression, ExpressionNodeType type)
        {
            Expression rdo;
            string content;
            PropertyInfo pi;

            content = expression.FormattedText ?? expression.Text;
            switch (type)
            {
                case ExpressionNodeType.StringType:
                    if (content.StartsWith("'") && content.EndsWith("'"))
                        content = content.Substring(1, content.Length - 2);

                    rdo = Expression.Constant(content);
                    break;

                case ExpressionNodeType.DateType:
                    if (content.StartsWith("#") && content.EndsWith("#"))
                        content = content.Substring(1, content.Length - 2);

                    rdo = Expression.Constant(DateTime.Parse(content, CultureInfo.InvariantCulture));
                    break;

                case ExpressionNodeType.PropertyType:
                    if (content.StartsWith("[") && content.EndsWith("]"))
                        content = content.Substring(1, content.Length - 2);

                    pi = ItemType.GetCachedProperties()[content];
                    rdo = Expression.Property(this.ParameterExpression, pi);
                    break;

                case ExpressionNodeType.UnknownType:
                    Double dbl;
                    bool bln;

                    // Adivinar tipo. Probar número, boolean, propiedad... Al final, si no se identifica, dejar como object.
                    if (Double.TryParse(content, NumberStyles.Number, CultureInfo.InvariantCulture, out dbl))
                        rdo = Expression.Constant(dbl);

                    else if (Boolean.TryParse(content, out bln))
                        rdo = Expression.Constant(bln);

                    else if (Properties.TryGetValue(content, out pi))
                        rdo = BuildValueExpression(expression, ExpressionNodeType.PropertyType);

                    else
                        rdo = Expression.Constant(content, typeof(object)); // TODO: Mas averiguaciones.

                    break;

                default:
                    throw new NotImplementedException(); // TODO: Tipo no permitido.
            }
            return rdo;
        }

        private Expression BuildArrayExpression(ExpressionNode expression)
        {
            var values = expression.Nodes
                .Select(BuildValueExpression)
                .ToArray();
            var types = values
                .Select(x => x.Type)
                .Distinct();
            var type = types.Count() == 1 ? types.First() : typeof(object);

            return Expression.NewArrayInit(type, values
                .Select(x => x.Type == type ? x : Expression.Convert(x, type))
                .ToArray());
        }

        private static Expression RelationalExpression(Expression expr1, ExpressionOperatorType @operator, Expression expr2)
        {
            Expression rdo;
            var primaryExpr = GetPrimaryOrSecondaryExpression(expr1, expr2, primary: true);
            var secondaryExpr = GetPrimaryOrSecondaryExpression(expr1, expr2, primary: false);

            if (@operator == ExpressionOperatorType.In)
            {
                rdo = RelationalExpressionIn(primaryExpr, secondaryExpr);
            }
            else
            {
                // Obtener la parte de la expressión que debería adaptar su tipo a la principal.
                if (primaryExpr.Type != secondaryExpr.Type)
                {
                    if (expr1 == secondaryExpr)
                        expr1 = Expression.Convert(secondaryExpr, primaryExpr.Type);
                    else
                        expr2 = Expression.Convert(secondaryExpr, primaryExpr.Type);
                }

                switch (@operator)
                {
                    case ExpressionOperatorType.Equals:
                        if (expr1.Type == typeof(string) && expr2.Type == typeof(string))
                            rdo = Expression.Call(stringEqualsMethod,
                                expr1, expr2, Expression.Constant(StringComparison.CurrentCultureIgnoreCase));
                        else
                            rdo = Expression.Equal(expr1, expr2);
                        break;
                    case ExpressionOperatorType.NotEqual:
                        rdo = Expression.NotEqual(expr1, expr2);
                        break;
                    case ExpressionOperatorType.GreaterThan:
                        rdo = Expression.GreaterThan(expr1, expr2);
                        break;
                    case ExpressionOperatorType.GreaterThanOrEqual:
                        rdo = Expression.GreaterThanOrEqual(expr1, expr2);
                        break;
                    case ExpressionOperatorType.LessThan:
                        rdo = Expression.LessThan(expr1, expr2);
                        break;
                    case ExpressionOperatorType.LessThanOrEqual:
                        rdo = Expression.LessThanOrEqual(expr1, expr2);
                        break;
                    case ExpressionOperatorType.Like:
                        rdo = Expression.Call(stringLikeMethod,
                            expr1, expr2,
                            Expression.New(typeof(StringLikeOptions)));
                        break;
                    default:
                        throw new InvalidOperationException(@operator.ToString()); // TODO: Mejorar error.
                }
            }
            return rdo;
        }

        private static Expression RelationalExpressionIn(Expression primaryExpr, Expression secondaryExpr)
        {
            Expression expr1, expr2;

#if PORTABLE
            throw new NotSupportedException($"nameof(RelationalExpressionIn) not available for this framework version.");
#else
            // Obtener la parte de la expressión que debería adaptar su tipo a la principal.
            if (primaryExpr.Type.BaseType == typeof(System.Array))
            {
                var primaryType = primaryExpr.Type.GetElementType();

                expr1 = (primaryType == secondaryExpr.Type ? secondaryExpr :
                    Expression.Convert(secondaryExpr, primaryType));
                expr2 = primaryExpr;
            }
            else if (secondaryExpr.Type.BaseType == typeof(System.Array))
            {
                var secondaryType = secondaryExpr.Type.GetElementType();

                expr1 = (secondaryType == primaryExpr.Type ? primaryExpr :
                    Expression.Convert(primaryExpr, secondaryType));
                expr2 = secondaryExpr;
            }
            else
            {
                throw new InvalidOperationException(nameof(RelationalExpressionIn)); // TODO: Mejorar error.
            }

            // TODO: cómo hacer compatible con IQueriable?
            MethodInfo containsMethod =
                typeof(System.Linq.Enumerable)
                    .GetMethodExt(nameof(System.Linq.Enumerable.Contains),
                        new Type[] { expr1.Type, typeof(IEnumerable<>).MakeGenericType(expr1.Type), expr1.Type });
            return Expression.Call(containsMethod, expr2, expr1);
#endif
        }

        private static Expression ArithmeticExpression(Expression expr1, ExpressionOperatorType @operator, Expression expr2)
        {
            Expression rdo;

            switch (@operator)
            {
                case ExpressionOperatorType.Add:
                    rdo = Expression.Add(expr1, expr2);
                    break;
                case ExpressionOperatorType.Substract:
                    rdo = Expression.Subtract(expr1, expr2);
                    break;
                case ExpressionOperatorType.Multiply:
                    rdo = Expression.Multiply(expr1, expr2);
                    break;
                case ExpressionOperatorType.Divide:
                    rdo = Expression.Divide(expr1, expr2);
                    break;
                case ExpressionOperatorType.Modulo:
                    rdo = Expression.Modulo(expr1, expr2);
                    break;
                case ExpressionOperatorType.Power:
                    rdo = Expression.Power(expr1, expr2);
                    break;
                default:
                    throw new InvalidOperationException(@operator.ToString()); // TODO: Mejorar error.
            }
            return rdo;
        }

        /// <summary>
        /// Returns which of expressions contains the primary data <see cref="Type"/>, according to <paramref name="primary"/> parameter.
        /// </summary>
        /// <param name="expr1">First expression.</param>
        /// <param name="expr2">Second expression.</param>
        /// <param name="primary">True to returns the expression that contains the primary <see cref="Type"/>. False to returns the expression that contains the secondary <see cref="Type"/>.</param>
        /// <returns>The expressions contains the primary data <see cref="Type"/>, according to <paramref name="primary"/> parameter.</returns>
        /// <remarks>
        /// Primary data type is every expression whose type is not <see cref="ExpressionType.Constant"/>.
        /// If both expressions are <see cref="ConstantExpression"/>, the funcion returns different one for each <paramref name="primary"/> value.
        /// </remarks>
        private static Expression GetPrimaryOrSecondaryExpression(Expression expr1, Expression expr2, bool primary)
        {
            Expression rdo = null;

            if (primary)
            {
                rdo = (expr1.NodeType == ExpressionType.Constant ? expr2 : expr1);
            }
            else
            {
                rdo = (expr1.NodeType == ExpressionType.Constant ? expr1 : expr2);
            }
            return rdo;
        }

        #endregion

    }
}