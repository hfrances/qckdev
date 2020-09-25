using System;
using System.Linq.Expressions;
using System.Reflection;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Provides an <see cref="Expression"/> generator from <see cref="ExpressionTree"/> items.
    /// </summary>
    public static class ExpressionBuilder
    {
        private static MethodInfo GetMethod(Type type, string name, Type[] types)
#if PORTABLE
            => type.GetRuntimeMethod(name, types);
#else
            => type.GetMethod(name, types);
#endif

        internal static readonly MethodInfo StringEqualsMethod =
            GetMethod(typeof(System.String), nameof(System.String.Equals),
                new Type[] { typeof(string), typeof(string), typeof(StringComparison) });
        internal static readonly MethodInfo StringLikeMethod =
            GetMethod(typeof(qckdev.Helper), nameof(qckdev.Helper.Like),
                new Type[] { typeof(string), typeof(string), typeof(StringLikeOptions) });


        /// <summary>
        /// Creates a <see cref="Expression"/> that represents an equality comparison between a <see cref="System.Reflection.PropertyInfo"/> and a constant.
        /// </summary>
        /// <typeparam name="T">The object <see cref="Type"/> that contains the <paramref name="propertyName"/></typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">The property type.</param>
        /// <param name="value">The constant value.</param>
        public static Expression<Func<T, bool>> PropertyEqual<T>(string propertyName, Type propertyType, object value)
        {
            var builder = new ExpressionBuilder<T, bool>();
            var propertyExpr = Expression.Property(builder.ParameterExpression, propertyName);
            var valueExpr = ExpressionHelper.Constant(value, propertyType);
            var equalExpr = Expression.Equal(propertyExpr, valueExpr);

            return builder.Lambda(equalExpr);
        }

        /// <summary>
        /// Creates a <see cref="Expression"/> that represents a <see cref="System.Reflection.PropertyInfo"/>. 
        /// This expression can be compiled and is faster than reflection access.
        /// </summary>
        /// <typeparam name="T">The object <see cref="Type"/> that contains the <paramref name="propertyName"/></typeparam>
        /// <typeparam name="TResult">The <see cref="Type"/> returned for the <see cref="Expression"/>.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        public static Expression<Func<T, TResult>> PropertyValue<T, TResult>(string propertyName)
        {
            var builder = new ExpressionBuilder<T, TResult>();
            var propertyExpr = Expression.Property(builder.ParameterExpression, propertyName);
            var convertedExpr = Expression.Convert(propertyExpr, typeof(TResult));

            return builder.Lambda(convertedExpr);
        }

    }
}
