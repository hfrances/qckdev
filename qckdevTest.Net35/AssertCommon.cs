using System;
using UnitTesting = Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    sealed class AssertCommon : Common.IAssert
    {
        public void AreEqual(object expected, object actual, string message = null)
            => UnitTesting.Assert.AreEqual(expected, actual, message);

        public void IsTrue(bool condition, string message = null)
            => UnitTesting.Assert.IsTrue(condition, message);

        public void IsFalse(bool condition, string message = null)
            => UnitTesting.Assert.IsFalse(condition, message);

        public void IsNull(object value, string message = null)
            => UnitTesting.Assert.IsNull(value, message);

        public void IsNotNull(object value, string message = null)
            => UnitTesting.Assert.IsNotNull(value, message);

        public void Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
                UnitTesting.Assert.Fail("Expected exception: " + typeof(TException).FullName);
            }
            catch (TException)
            {
            }
        }
    }
}
