using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace qckdev.Linq
{

    /// <summary>
    /// Provides a set of static methods for querying objects that implement <see cref="IList"/>.
    /// </summary>
    public static class List
    {

        /// <summary>
        /// Adds the elements of an <see cref="ICollection"/> to the end of the <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="c">
        /// The ICollection whose elements should be added to the end of the <paramref name="list"/>. 
        /// The collection itself cannot be null, but it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="c"/> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <paramref name="list"/> is read-only 
        /// -or- 
        /// The <paramref name="list"/> has a fixed size.</exception>
        public static void AddRange(this IList list, ICollection c)
        {
            if (!(c is IList l))
            {
                var count = c.Count;
                var array = new object[count];

                c.CopyTo(array, 0);
                for (int i = 0; i < count; i++)
                    list.Add(array[i]);
            }
            else
            {
                AddRange(list, l);
            }
        }

        /// <summary>
        /// Adds the elements of an <see cref="IList"/> to the end of the <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="l">
        /// The ICollection whose elements should be added to the end of the <paramref name="list"/>. 
        /// The collection itself cannot be null, but it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="l"/> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <paramref name="list"/> is read-only 
        /// -or- 
        /// The <paramref name="list"/> has a fixed size.</exception>
        [SuppressMessage("Sonar Code Smell", "S3242:Method parameters should be declared with base types", Justification = "l[index]")]
        public static void AddRange(this IList list, IList l)
        {
            var count = l.Count;

            for (int i = 0; i < count; i++)
                list.Add(l[i]);
        }

        /// <summary>
        /// Replaces the first occurrence of a specific object from the <see cref="ICollection{T}"/> for new one.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="oldValue">The object to replace from the <see cref="ICollection{T}"/>.</param>
        /// <param name="newValue">The new object.</param>
        public static bool TryReplace<T>(this IList<T> list, T oldValue, T newValue)
        {
            var rdo = false;
            var idx = list.IndexOf(oldValue);

            if (idx > -1)
            {
                list[idx] = newValue;
                rdo = true;
            }
            return rdo;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item">The object to remove from the <see cref="IList{T}"/>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="IList{T}"/>;
        /// otherwise, false. This method also returns false if item is not found in the
        /// original <see cref="ICollection{T}"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">The <see cref="IList{T}"/> is read-only </exception>
        public static bool TryRemove<T>(this IList<T> list, T item)
        {
            var rdo = false;
            var idx = list.IndexOf(item);

            if (idx > -1)
            {
                list.RemoveAt(idx);
                rdo = true;
            }
            return rdo;
        }

    }
}
