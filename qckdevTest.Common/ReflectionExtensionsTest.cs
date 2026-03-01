using qckdev.Reflection;
using qckdevTest.Common.TestObjects;
using System.Reflection;

namespace qckdevTest.Common
{
    public sealed class ReflectionExtensionsTest
    {
        readonly IAssert Assert;

        public ReflectionExtensionsTest(IAssert assert)
        {
            Assert = assert;
        }

        public void HasOwnValue_CoversPrimitive_String_Class()
        {
            Assert.IsTrue(typeof(int).HasOwnValue());
            Assert.IsTrue(typeof(string).HasOwnValue());
            Assert.IsFalse(typeof(ReflectionFactoryItems.PlainObject).HasOwnValue());
        }

        public void GetMethodExt_WithBindingFlags_FindsNonPublicMethod()
        {
            var method = typeof(PrivateMethodCarrier).GetMethodExt(
                "Hidden",
                BindingFlags.Instance | BindingFlags.NonPublic,
                typeof(int));

            Assert.IsNotNull(method);
        }

        sealed class PrivateMethodCarrier
        {
            int Hidden(int x) => x;
        }
    }
}
