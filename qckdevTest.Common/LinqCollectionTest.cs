using System.Collections.Generic;
using System.Linq;

namespace qckdevTest.Common
{
    public sealed class LinqCollectionTest
    {
        readonly IAssert Assert;

        public LinqCollectionTest(IAssert assert)
        {
            Assert = assert;
        }

        public void AddRange_And_IndexOf()
        {
            var list = new List<int>() { 1, 2 };
            var linked = new LinkedList<int>(new[] { 3, 4 });

            qckdev.Linq.Collection.AddRange(list, linked);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(2, qckdev.Linq.Collection.IndexOf(list, x => x == 3));
            Assert.AreEqual(-1, qckdev.Linq.Collection.IndexOf(list, x => x == 99));
        }
    }
}
