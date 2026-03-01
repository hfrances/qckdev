using System;

namespace qckdevTest.Common
{
    public sealed class LinqEnumerableExtensionsTest
    {
        readonly IAssert Assert;

        public LinqEnumerableExtensionsTest(IAssert assert)
        {
            Assert = assert;
        }

        public void ForEach_ExecutesAction()
        {
            var sum = 0;
            qckdev.Linq.Enumerable.ForEach(new[] { 1, 2, 3 }, x => sum += x);
            Assert.AreEqual(6, sum);
        }

        public void ForEach_Throws_WhenActionNull()
        {
            Assert.Throws<NullReferenceException>(() => qckdev.Linq.Enumerable.ForEach(new[] { 1 }, null));
        }
    }
}
