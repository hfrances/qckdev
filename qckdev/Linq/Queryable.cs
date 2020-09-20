#if STANDARD12 // EXCLUDE.
#else

using qckdev.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace qckdev.Linq
{

    /// <summary>
    /// Provides a set of static methods for querying objects that implement <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class Queryable
    {

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. 
        /// A specified <see cref="IEqualityComparer{T}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">
        /// The type of the elements of the first sequence.
        /// </typeparam>
        /// <typeparam name="TInner">
        /// The type of the elements of the second sequence.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the keys returned by the key selector functions.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result elements.
        /// </typeparam>
        /// <param name="outer">
        /// The first sequence to join.
        /// </param>
        /// <param name="inner">
        /// The sequence to join to the first sequence.
        /// </param>
        /// <param name="outerKeySelector">
        /// A function to extract the join key from each element of the first sequence.
        /// </param>
        /// <param name="innerKeySelector">
        /// A function to extract the join key from each element of the second sequence.
        /// </param>
        /// <param name="resultSelector">
        /// A function to create a result element from two matching elements.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> 
        /// that has elements of type <typeparamref name="TResult"/> 
        /// that are obtained by performing an inner join on two sequences.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Outer or inner or outerKeySelector or innerKeySelector or resultSelector is null.
        /// <typeparamref name="TOuter"/> or 
        /// <typeparamref name="TInner"/> or 
        /// <paramref name="outerKeySelector"/> or 
        /// <paramref name="innerKeySelector"/> or 
        /// <paramref name="resultSelector"/> is null.
        /// </exception>
        [SuppressMessage("Minor Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "All parameters are necessary.")]
        public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer, IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            var query =
                SelectMany(
                    System.Linq.Queryable.GroupJoin(
                        outer,
                        inner,
                        outerKeySelector,
                        innerKeySelector,
                        (x, y) => new { Outer = x, Details = y }
                    ),
                    z => System.Linq.Enumerable.DefaultIfEmpty(z.Details),
                    resultSelector
                );
            return query;
        }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. 
        /// A specified <see cref="IEqualityComparer{T}"/> is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">
        /// The type of the elements of the first sequence.
        /// </typeparam>
        /// <typeparam name="TInner">
        /// The type of the elements of the second sequence.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the keys returned by the key selector functions.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result elements.
        /// </typeparam>
        /// <param name="outer">
        /// The first sequence to join.
        /// </param>
        /// <param name="inner">
        /// The sequence to join to the first sequence.
        /// </param>
        /// <param name="outerKeySelector">
        /// A function to extract the join key from each element of the first sequence.
        /// </param>
        /// <param name="innerKeySelector">
        /// A function to extract the join key from each element of the second sequence.
        /// </param>
        /// <param name="resultSelector">
        /// A function to create a result element from two matching elements.
        /// </param>
        /// <param name="comparer">
        /// An <see cref="IEqualityComparer{T}"/> to hash and compare keys.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> 
        /// that has elements of type <typeparamref name="TResult"/> 
        /// that are obtained by performing an inner join on two sequences.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Outer or inner or outerKeySelector or innerKeySelector or resultSelector is null.
        /// <typeparamref name="TOuter"/> or 
        /// <typeparamref name="TInner"/> or 
        /// <paramref name="outerKeySelector"/> or 
        /// <paramref name="innerKeySelector"/> or 
        /// <paramref name="resultSelector"/> is null.
        /// </exception>
        [SuppressMessage("Minor Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "All parameters are necessary.")]
        public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
                this IQueryable<TOuter> outer,
                IQueryable<TInner> inner,
                Expression<Func<TOuter, TKey>> outerKeySelector,
                Expression<Func<TInner, TKey>> innerKeySelector,
                Expression<Func<TOuter, TInner, TResult>> resultSelector,
                IEqualityComparer<TKey> comparer)
        {

            var query =
                SelectMany(
                    System.Linq.Queryable.GroupJoin(
                        outer,
                        inner,
                        outerKeySelector,
                        innerKeySelector,
                        (x, y) => new { Outer = x, Details = y },
                        comparer
                    ),
                    z => System.Linq.Enumerable.DefaultIfEmpty(z.Details),
                    resultSelector
                );
            return query;
        }

        [SuppressMessage("Minor Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "All parameters are necessary.")]
        private static IQueryable<TResult> SelectMany<TSource, TCollection, TInner, TOuter, TResult>(
            IQueryable<TSource> source,
            Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            var group = Expression.Parameter(typeof(TSource), "group");
            var outer = Expression.Property(group, "Outer");
            var inner = Expression.Parameter(typeof(TCollection), "inner");
            var body = resultSelector.Body
                .ReplaceParameter(resultSelector.Parameters[0], outer)
                .ReplaceParameter(resultSelector.Parameters[1], inner);

            var lambda = Expression.Lambda<Func<TSource, TCollection, TResult>>(body, group, inner);
            return System.Linq.Queryable.SelectMany(
                source, collectionSelector, lambda);
        }
    }

}

#endif