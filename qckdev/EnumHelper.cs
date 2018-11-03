using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace qckdev
{

    static class EnumHelper
    {

        public static bool ContainsAll<T>(T value, params T[] others)
        {
            bool rdo = (others.Length > 0);
            var v = (int)Enum.ToObject(typeof(T), value);

            for (int i = 0; i < others.Length && rdo; i++)
            {
                var o = (int)Enum.ToObject(typeof(T), others[i]);
                rdo = (v & o) > 0;
            }
            return rdo;
        }

    }
}
