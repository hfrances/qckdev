using System;
using System.Linq;
using System.Collections.Generic;

namespace qckdev.Linq
{

    /// <summary>
    /// Provides a set of static methods for querying objects that implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class Enumerable
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
        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return LeftJoin<TOuter, TInner, TKey, TResult>(outer, 
                inner, outerKeySelector, innerKeySelector, resultSelector, null);
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
        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer, 
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer) => outer
                .GroupJoin(inner,
                    outerKeySelector,
                    innerKeySelector,
                    (x, y) => new { x, y },
                    comparer)
                .SelectMany(
                    m => m.y.DefaultIfEmpty(),
                    (m, y) => resultSelector(m.x, y));

        /// <summary>
        /// Performs the specified action on each element of the <see cref="System.Collections.Generic.IEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable">Elements where apply the action.</param>
        /// <param name="action">The <see cref="System.Action{T}"/> delegate to perform on each element of the <see cref="System.Collections.Generic.IEnumerable{T}"/>.</param>
        /// <exception cref="System.ArgumentNullException">action is null.</exception>
        /// <exception cref="System.InvalidOperationException">An element in the collection has been modified.</exception>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action.Invoke(item);
            }
        }

    }
}
