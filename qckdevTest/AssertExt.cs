using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace qckdevTest
{
    static class AssertExt
    {

        [SuppressMessage("Critical Code Smell", "S1125:Remove unnecessary Boolean literal(s).", Justification = "Make sure that assignation in condition sentence is right.")]
        public static void AreEqual(IEnumerable expected, IEnumerable actual)
        {

            int expectedIndex = 0, actualIndex = 0;
            bool expectedNext = false, actualNext = false;
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
