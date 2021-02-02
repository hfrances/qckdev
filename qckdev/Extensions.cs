using System;
using System.Text;

namespace qckdev
{

    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods.
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Determines whether a specified value matches any value in a a list.
        /// </summary>
        /// <typeparam name="T">The type of elements to validate.</typeparam>
        /// <param name="this">Value to validate.</param>
        /// <param name="values">List of values to compare.</param>
        /// <returns>Returns true if one of list values matches with the specified value.</returns>
        public static bool In<T>(this T @this, params T[] values)
        {
            bool found = false;
            int index = 0;

            while (index < values.Length && !found)
            {
                found = object.Equals(@this, values[index]);
                index += 1;
            }
            return found;
        }

        /// <summary>
        /// Determines whether a specified value matches any value in a a list.
        /// </summary>
        /// <param name="this">Value to validate.</param>
        /// <param name="ignoreCase">Ignores case sensitive.</param>
        /// <param name="values">List of values to compare.</param>
        /// <returns>Returns true if one of list values matches with the specified value.</returns>
        public static bool In(this string @this, bool ignoreCase, params string[] values)
        {
            bool found = false;
            int index = 0;

            while (index < values.Length && !found)
            {
                found = string.Equals(@this, values[index], (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
                index += 1;
            }
            return found;
        }

        /// <summary>
        /// Clears string builder content.
        /// </summary>
        /// <param name="value">
        /// The <see cref="StringBuilder"/> to clear.
        /// </param>
        public static void Clear(this StringBuilder value)
        {
            value.Length = 0;
            value.Capacity = 0;
        }

    }
}
