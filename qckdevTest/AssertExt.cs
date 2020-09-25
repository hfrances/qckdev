using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace qckdevTest
{
    static class AssertExt
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

        [SuppressMessage("Critical Code Smell", "S1125:Remove unnecessary Boolean literal(s).", Justification = "Make sure that assignation in condition sentence is right.")]
        public static void AreEqual(IEnumerable expected, IEnumerable actual)
        {

            int expectedIndex = 0, actualIndex = 0;
            bool expectedNext;
            bool actualNext = false;
            var expectedEtor = expected.GetEnumerator();
            var actualEtor = actual.GetEnumerator();

            while (true == (expectedNext = expectedEtor.MoveNext())
                || true == (actualNext = actualEtor.MoveNext()))
            {
                if (expectedNext)
                    expectedIndex++;
                if (actualNext)
                    actualIndex++;

                if (expectedNext && actualNext)
                {
                    Assert.AreEqual(expectedEtor.Current, actualEtor.Current, $"Index {expectedIndex}");
                }
            }
            Assert.AreEqual(expectedIndex, actualIndex, $"Item count does not equal.");
        }

    }
}
