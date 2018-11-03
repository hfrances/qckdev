using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qckdev.Linq;

namespace qckdevTest.Linq
{

    [TestClass]
    public class EnumerableTest
    {

        [TestMethod]
        public void LeftJoin01()
        {
            var parent = new[] {
                new { Id = 1 },
                new { Id = 2 }
            };
            var child = new[] {
                new {Id = 1, Linea = 1},
                new {Id = 1, Linea = 2},
                new {Id = 3, Linea = 1}
            };
            var qry = parent
                .LeftJoin(child, x => x.Id, y => y.Id,
                    (x, y) => new
                    {
                        Parent = x,
                        Childs = y
                    })
                .ToArray();

            if (qry.Any())
            {
                var qry1 = qry.Where(x => x.Parent.Id == 1).ToArray();
                var qry2 = qry.Where(x => x.Parent.Id == 2).ToArray();
                var qry3 = qry.Where(x => x.Parent.Id == 3).ToArray();

                Assert.IsTrue(
                    qry1.Length == 2 &&
                        qry1[0].Childs.Linea == 1 &&
                        qry1[1].Childs.Linea == 2,
                    "Parent - Child");
                Assert.IsTrue(
                    qry2.Length == 1 &&
                        qry2[0].Childs == null,
                    "Parent - null");
                Assert.IsTrue(
                    qry3.Length == 0,
                    "null - Child");
            }
            else
            {
                Assert.Fail("No results.");
            }
        }

        [TestMethod]
        [DataRow(true, DisplayName = "IgnoreCase = true")]
        [DataRow(false, DisplayName = "IgnoreCase = false")]
        public void LeftJoin_IgnoreCase(bool value)
        {
            var parent = new[] {
                new { Id = "A" },
                new { Id = "B" }
            };
            var child = new[] {
                new {Id = "A", Linea = 1},
                new {Id = "a", Linea = 2},
                new {Id = "C", Linea = 1}
            };
            var qry = parent
                .LeftJoin(child, x => x.Id, y => y.Id,
                    (x, y) => new
                    {
                        Parent = x,
                        Childs = y
                    },
                 (value ? StringComparer.InvariantCultureIgnoreCase : null))
                .ToArray();

            if (qry.Any())
            {
                var qry1 = qry.Where(x => x.Parent.Id == "A").ToArray();
                var qry2 = qry.Where(x => x.Parent.Id == "B").ToArray();
                var qry3 = qry.Where(x => x.Parent.Id == "C").ToArray();

                if (value)
                {
                    Assert.IsTrue(
                        qry1.Length == 2 &&
                            qry1[0].Childs.Linea == 1 &&
                            qry1[1].Childs.Linea == 2,
                        "Parent - Child");
                }
                else
                {
                    Assert.IsTrue(
                        qry1.Length == 1 &&
                            qry1[0].Childs.Linea == 1,
                        "Parent - Child");
                }
                Assert.IsTrue(
                    qry2.Length == 1 &&
                        qry2[0].Childs == null,
                    "Parent - null");
                Assert.IsTrue(
                    qry3.Length == 0,
                    "null - Child");
            }
            else
            {
                Assert.Fail("No results.");
            }
        }

    }
}
