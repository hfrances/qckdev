using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qckdevTest
{
    static class Assert2
    {

        public const string DBNullCONST = "1B544BD7-608D-438F-B6DC-F3A1B538A898";

        public static void AreEqualDBNull(object expected, object actual)
        {
            if (object.Equals(expected, DBNullCONST))
                Assert.AreEqual(DBNull.Value, actual);
            else
                Assert.AreEqual(expected, actual);
        }

        public static void AreNotEqualDBNull(object expected, object actual)
        {
            if (object.Equals(expected, DBNullCONST))
                Assert.AreNotEqual(DBNull.Value, actual);
            else
                Assert.AreNotEqual(expected, actual);
        }

    }
}
