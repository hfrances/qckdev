using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class SharedTest
    {
        readonly Common.SharedTest Inner = new Common.SharedTest(new AssertCommon());

        [TestMethod] public void Choose_UsesConflictAction_WhenBothValuesExist() => Inner.Choose_UsesConflictAction_WhenBothValuesExist();
        [TestMethod] public void Choose_ReturnsNonNullValue_WhenOtherIsNull() => Inner.Choose_ReturnsNonNullValue_WhenOtherIsNull();
    }
}
