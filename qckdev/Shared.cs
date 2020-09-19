using System;

namespace qckdev
{

    /// <summary>
    /// Provides a set of miscelaneous static (Shared in Visual Basic) methods.
    /// </summary>
    static class Shared
    {

        public static T Choose<T>(T val1, T val2, Func<T, T, T> conflictAction) where T : class
        {
            T rdo;

            if (val1 != null && val2 != null)
                rdo = conflictAction.Invoke(val1, val2);
            else if (val1 == null)
                rdo = val2;
            else
                rdo = val1;
            return rdo;
        }

    }
}
