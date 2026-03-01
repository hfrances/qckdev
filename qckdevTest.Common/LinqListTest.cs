using System.Collections;
using System.Collections.Generic;

namespace qckdevTest.Common
{
    public sealed class LinqListTest
    {
        readonly IAssert Assert;

        public LinqListTest(IAssert assert)
        {
            Assert = assert;
        }

        public void AddRange_TryReplace_TryRemove()
        {
            var rawList = (IList)new ArrayList();
            var queue = new Queue();
            queue.Enqueue("a");
            queue.Enqueue("b");
            qckdev.Linq.List.AddRange(rawList, queue);
            qckdev.Linq.List.AddRange(rawList, (IList)new ArrayList() { "c" });
            Assert.AreEqual(3, rawList.Count);

            var list = new List<string>() { "x", "y", "z" };
            Assert.IsTrue(qckdev.Linq.List.TryReplace(list, "y", "Y"));
            Assert.AreEqual("Y", list[1]);
            Assert.IsFalse(qckdev.Linq.List.TryReplace(list, "missing", "M"));
            Assert.IsTrue(qckdev.Linq.List.TryRemove(list, "Y"));
            Assert.IsFalse(qckdev.Linq.List.TryRemove(list, "missing"));
        }
    }
}
