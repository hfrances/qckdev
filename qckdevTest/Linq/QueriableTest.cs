using Microsoft.VisualStudio.TestTools.UnitTesting;
using qckdev.Linq;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace qckdevTest.Linq
{

    [TestClass]
    public class QueriableTest
    {

        [TestMethod]
        public void LeftJoinTest()
        {

            using (var context = TestObjects.TestDbContext.CreateInstance())
            {
                InitializeData(context);

                var expected = new[]
                {
                    new { Number = 1, Line = (int?)1 },
                    new { Number = 1, Line = (int?)2 },
                    new { Number = 2, Line = (int?)1 },
                    new { Number = 3, Line = (int?)null },

                };

                var actual =
                    context.Parents
                        .LeftJoin(context.Childs,
                            x => x.Id,
                            y => y.Id,
                            (x, y) => new
                            {
                                x.Number,
                                y?.Line,
                            })
                        .OrderBy(x => x.Number)
                            .ThenBy(x => x.Line)
                        .ToArray();

                AssertAreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void GroupJoinTest()
        {

            using (var context = TestObjects.TestDbContext.CreateInstance())
            {
                InitializeData(context);

                var result =
                    context.Parents
                        .GroupJoin(context.Childs,
                            x => x.Id,
                            y => y.Id,
                            (x, y) => x);

                var resultArray = result.ToArray();
                Assert.Inconclusive();
            }
        }

        [SuppressMessage("Critical Code Smell", "S1125:Remove unnecessary Boolean literal(s).", Justification = "Make sure that assignation in condition sentence is right.")]
        private static void AssertAreEqual(IEnumerable expected, IEnumerable actual)
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

        private static void InitializeData(TestObjects.TestDbContext context)
        {
            Guid id1 = Guid.NewGuid(), id2 = Guid.NewGuid(), id3 = Guid.NewGuid();

            context.AddRange(
                new TestObjects.Entities.Parent { Id = id1, Number = 1, Name = "Item 1" },
                new TestObjects.Entities.Parent { Id = id2, Number = 2, Name = "Item 2" },
                new TestObjects.Entities.Parent { Id = id3, Number = 3, Name = "Item 2" },
                new TestObjects.Entities.Child { Id = id1, Line = 1, Description = "Line 1" },
                new TestObjects.Entities.Child { Id = id1, Line = 2, Description = "Item 2", Remarks = "With remarks" },
                new TestObjects.Entities.Child { Id = id2, Line = 1, Description = "Line 1" },
                new TestObjects.Entities.Orphan { Id = id1, Line = 1 },
                new TestObjects.Entities.Orphan { Id = id1, Line = 2 },
                new TestObjects.Entities.Orphan { Id = id2, Line = 1 }
            );
            context.SaveChanges();
        }

    }
}
