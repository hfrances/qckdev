using System.Collections.Generic;

namespace qckdev.Reflection
{

    /// <summary>
    /// Provides helpful methods for reflection. This class cannot be inherited.
    /// </summary>
    public static partial class ReflectionHelper
    {

        /// <summary>
        /// Serves as a hash function for a object list.
        /// </summary>
        /// <param name="values">List of elements for HashCode creation.</param>
        /// <returns>A hash code for the object list.</returns>
        public static int GetHashCode(params object[] values)
        {
            return GetHashCode((IEnumerable<object>)values);
        }

        /// <summary>
        /// Serves as a hash function for a object list.
        /// </summary>
        /// <param name="values">List of elements for HashCode creation.</param>
        /// <returns>A hash code for the object list.</returns>
        public static int GetHashCode(IEnumerable<object> values)
        {
            int rdo = 0;

            foreach (var value in values)
            {
                rdo ^= (int)(uint)(value?.GetHashCode() ?? 0);
            }
            return rdo;
        }

    }

}
