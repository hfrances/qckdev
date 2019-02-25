using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Provides helpful methods for Linq expressions. This class cannot be inherited.
    /// </summary>
    public static class ExpressionHelper
    {

#if PORTABLE
        private static readonly MethodInfo convertMethod = typeof(Convert).GetRuntimeMethod(nameof(Convert.ChangeType), new Type[] { typeof(object), typeof(Type) });
#else
        private static readonly MethodInfo convertMethod = typeof(Convert).GetMethod(nameof(Convert.ChangeType), new Type[] { typeof(object), typeof(Type) });
#endif

        /// <summary>
        /// Creates a <see cref="ConstantExpression"/> with nullable support.
        /// </summary>
        /// <param name="value">Constant value.</param>
        /// <param name="type">Constant type. Can be a <see cref="Nullable{T}"/> type.</param>
        /// <returns></returns>
        public static ConstantExpression Constant(object value, Type type)
        {
            var valueType = (value?.GetType() ?? type);

            if (type != valueType)
            {
                var underType = Nullable.GetUnderlyingType(type);

                // Tipo diferente, hacer conversiones.
                if (underType == null)
                {
                    value = Convert.ChangeType(value, type); // Intentar convertir el tipo.
                }
                else
                {
                    value = Convert.ChangeType(value, underType); // Intentar convertir el tipo.
                    value = Activator.CreateInstance(type, value); // Pasar el valor convertido a objeto nullable.
                }
            }
            return Expression.Constant(value, type);
        }

        /// <summary>
        /// Creates a <see cref="MethodCallExpression"/> that returns an object of the specified type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="expression">An <see cref="Expression"/> that implements the System.IConvertible interface</param>
        /// <param name="conversionType">The type of object to return.</param>
        /// <returns></returns>
        public static MethodCallExpression ChangeType(this Expression expression, Type conversionType)
        {
            Convert.ChangeType(null, conversionType);
            return Expression.Call(convertMethod, expression, Expression.Constant(conversionType));
        }

        /// <summary>
        /// Concatenates several <see cref="Expression"/> elements.
        /// </summary>
        /// <param name="function">Function to invoke.</param>
        /// <param name="expressions">List of <see cref="Expression"/> to concatenate.</param>
        public static Expression Concatenate(Func<Expression, Expression, BinaryExpression> function, params Expression[] expressions)
        {
            return Concatenate(function, expressions.AsEnumerable());
        }

        /// <summary>
        /// Concatenates several <see cref="Expression"/> elements.
        /// </summary>
        /// <param name="function">Function to invoke.</param>
        /// <param name="expressions">List of <see cref="Expression"/> to concatenate.</param>
        public static Expression Concatenate(Func<Expression, Expression, BinaryExpression> function, IEnumerable<Expression> expressions)
        {
            Expression rdo = null;
            var eExprs = expressions.GetEnumerator();

            while (eExprs.MoveNext())
            {
                if (rdo == null)
                {
                    rdo = eExprs.Current;
                }
                else
                {
                    rdo = function(rdo, eExprs.Current);
                }
            }
            return rdo;
        }

        /// <summary>
        /// Replaces '*' and '?' for '%' and '_' excepting when they are preceded by '\' character.
        /// </summary>
        /// <param name="value">Value to format.</param>
        internal static string FormatToSQLFilter(string value)
        {
            var newValue = new StringBuilder();
            var bScape = false;

            for (int i = 0; i <= value.Length - 1; i++)
            {
                char current = value[i];

                if (current == '\\' && !bScape)
                {
                    bScape = true;
                }
                else
                {
                    switch (current)
                    {
                        case '?':
                            if (!bScape)
                                current = '_';
                            break;
                        case '*':
                            if (!bScape)
                                current = '%';
                            break;
                        case '\'':
                            break; // Continue
                        default:
                            if (bScape)
                                newValue.Append('\\');
                            break;
                    }
                    bScape = false;
                }
                if (!bScape)
                {
                    newValue.Append(current);
                }
            }
            return newValue.ToString();
        }

        /// <summary>
        /// Generates a <see cref="Regex"/> from a SQL LIKE format pattern.
        /// </summary>
        /// <param name="pattern">Value to format</param>
        internal static Regex LikeRegex(string pattern)
        {
            pattern = FormatToSQLFilter(pattern);
            pattern = pattern.Replace("''", "'"); // TODO: Esto debería ser para cualquier cadena.
            if (!pattern.EndsWith("%")) pattern += "%";

            return new Regex(@"\A" +
                   new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(pattern.Trim(), ch => @"\" +
                                Convert.ToString(ch)).Replace('_', '.').Replace(" % ", ".* ") + @"\z",
                                RegexOptions.Singleline);
        }

    }
}
