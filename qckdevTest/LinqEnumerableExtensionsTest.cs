using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class LinqEnumerableExtensionsTest
    {
        readonly Common.LinqEnumerableExtensionsTest Inner = new Common.LinqEnumerableExtensionsTest(new AssertCommon());

        [TestMethod] public void ForEach_ExecutesAction() => Inner.ForEach_ExecutesAction();
        [TestMethod] public void ForEach_Throws_WhenActionNull() => Inner.ForEach_Throws_WhenActionNull();
    }
}
