using qckdev.Reflection;
using qckdevTest.Common.TestObjects;
using System.Collections.Generic;
using System.Reflection;

namespace qckdevTest.Common
{
    public sealed class ReflectionHelperTest
    {
        readonly IAssert Assert;

        public ReflectionHelperTest(IAssert assert)
        {
            Assert = assert;
        }

        public void GetHashCode_HandlesNulls_AndOverloads()
        {
            var values = new object[] { 1, null, "A" };
            var h1 = ReflectionHelper.GetHashCode(values);
            var h2 = ReflectionHelper.GetHashCode((IEnumerable<object>)values);

            Assert.AreEqual(h1, h2);
        }

        public void CreateInstance_And_GetConstructor()
        {
            var pub = (ReflectionFactoryItems.PublicCtorClass)ReflectionHelper.CreateInstance(
                typeof(ReflectionFactoryItems.PublicCtorClass), "abc", 5);
            Assert.AreEqual("abc", pub.Name);
            Assert.AreEqual(5, pub.Number);

            var priv = ReflectionHelper.CreateInstance(typeof(ReflectionFactoryItems.PrivateCtorClass), "secret");
            Assert.IsNotNull(priv);

            var ctor = ReflectionHelper.GetConstructor(typeof(ReflectionFactoryItems.PublicCtorClass), "abc", 5);
            Assert.IsNotNull(ctor);

            var missing = ReflectionHelper.GetConstructor(typeof(ReflectionFactoryItems.NoMatchingCtorClass), "bad");
            Assert.IsNull(missing);
        }

        public void GetCallingMethod_ReturnsExpectedMethod()
        {
            var method = Level1_GetCallingMethod();
            Assert.AreEqual(nameof(Level1_GetCallingMethod), method.Name);
        }

        static MethodBase Level1_GetCallingMethod()
        {
            return Level2_GetCallingMethod();
        }

        static MethodBase Level2_GetCallingMethod()
        {
            return ReflectionHelper.GetCallingMethod();
        }
    }
}
