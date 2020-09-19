using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace qckdev.Linq
{

    /// <summary>
    /// Provides a set of static methods for querying objects that implement <see cref="ICollection{T}"/>.
    /// </summary>
    public static class Collection
    {

        /// <summary>
        /// Adds the elements of an <see cref="IEnumerable{T}"/> to the end of the <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="e">
        /// The ICollection whose elements should be added to the end of the <paramref name="collection"/>. 
        /// The collection itself cannot be null, but it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="e"/> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <paramref name="collection"/> is read-only 
        /// -or- 
        /// The <paramref name="collection"/> has a fixed size.</exception>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> e)
        {
            if (e is IList<T> l)
                AddRange(collection, l);
            else if (e is ICollection<T> c)
                AddRange(collection, c.ToArray());
            else
                AddRange(collection, e.ToArray());
        }

        private static void AddRange<T>(this ICollection<T> collection, T[] array)
        {
            var length = array.Length;

            for (int i = 0; i < length; i++)
                collection.Add(array[i]);
        }

        /// <summary>
        /// Adds the elements of an <see cref="IList{T}"/> to the end of the <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="l">
        /// The ICollection whose elements should be added to the end of the <paramref name="collection"/>. 
        /// The collection itself cannot be null, but it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="l"/> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <paramref name="collection"/> is read-only 
        /// -or- 
        /// The <paramref name="collection"/> has a fixed size.</exception>
        [SuppressMessage("Sonar Code Smell", "S3242:Method parameters should be declared with base types", Justification = "l[index]")]
        public static void AddRange<T>(this ICollection<T> collection, IList<T> l)
        {
            var count = l.Count;

            for (int i = 0; i < count; i++)
                collection.Add(l[i]);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="collection">An <see cref="ICollection{T}"/> to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The index of value if found in the collection; otherwise, -1.</returns>
        public static int IndexOf<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var rdo = -1;
            var count = collection.Count;
            var array = new T[count];

            collection.CopyTo(array, 0);
            for (int i = 0; i < count && rdo == 1; i++)
                if (predicate(array[i]))
                    rdo = i;
            return rdo;
        }

    }
}
