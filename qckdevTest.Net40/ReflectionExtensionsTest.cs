using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class ReflectionExtensionsTest
    {
        readonly Common.ReflectionExtensionsTest Inner = new Common.ReflectionExtensionsTest(new AssertCommon());

        [TestMethod] public void HasOwnValue_CoversPrimitive_String_Class() => Inner.HasOwnValue_CoversPrimitive_String_Class();
        [TestMethod] public void GetMethodExt_WithBindingFlags_FindsNonPublicMethod() => Inner.GetMethodExt_WithBindingFlags_FindsNonPublicMethod();
    }
}
