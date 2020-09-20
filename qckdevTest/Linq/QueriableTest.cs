using Microsoft.VisualStudio.TestTools.UnitTesting;
using qckdev.Linq;
using System;
using System.Linq;

namespace qckdevTest.Linq
{

    [TestClass]
    public class QueriableTest
    {

        [TestMethod]
        public void LeftJoin()
        {

            using (var context = TestObjects.TestDbContext.CreateInstance())
            {
                Guid id1 = Guid.NewGuid(), id2 = Guid.NewGuid(), id3 = Guid.NewGuid();

                context.AddRange(
                    new TestObjects.Entities.Parent { Id = id1, Number = 1, Name = "Item 1" },
                    new TestObjects.Entities.Parent { Id = id2, Number = 2, Name = "Item 2" },
                    new TestObjects.Entities.Parent { Id = id3, Number = 3, Name = "Item 2" },
                    new TestObjects.Entities.Child { Id = id1, Line = 1, Description = "Line 1" },
                    new TestObjects.Entities.Child { Id = id1, Line = 2, Description = "Item 2", Remarks = "With remarks" },
                    new TestObjects.Entities.Child { Id = id2, Line = 1, Description = "Line 1" }
                );
                context.SaveChanges();

                var result = 
                    context.Parents
                        .LeftJoin(context.Childs,
                            x => x.Id,
                            y => y.Id,
                            (x, y) => new
                            {
                                x.Number,
                                y?.Line,
                            })
                        .OrderBy(x=> x.Number)
                            .ThenBy(x=> x.Line)
                        .ToArray();

                var expected = new[]
                {
                    new { Number = 1, Line = (int?)1 },
                    new { Number = 1, Line = (int?)2 },
                    new { Number = 2, Line = (int?)1 },
                    new { Number = 3, Line = (int?)null },

                };

                Assert.AreEqual(expected.Length, result.Length, "Result lenght not equals.");
                if (expected.Length == result.Length)
                {
                    var index = 1;
                    var expectedEtor = expected.GetEnumerator();
                    var resultEtor = result.GetEnumerator();
                    while (expectedEtor.MoveNext() && resultEtor.MoveNext())
                    {
                        Assert.AreEqual(expectedEtor.Current, resultEtor.Current, $"Index {index}");
                        index++;
                    }
                }
            }
        }

    }
}
