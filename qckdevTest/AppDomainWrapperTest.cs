using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using qckdev;

namespace qckdevTest
{

    [TestClass]
    public class AppDomainWrapperTest
    {

        [TestMethod]
        [DataRow(typeof(System.Data.SqlClient.SqlConnection), DisplayName = "Check with Class.")]
        [DataRow(typeof(System.Drawing.SizeF), DisplayName = "Check with Structure.")]
        public void AppDomainWrapper_InstanceAndUnwrap(Type type)
        {
            object rdo;
            var appDomain = AppDomain.CreateDomain("TestDomain");

            using (var wrpDomain = new AppDomainWrapper(appDomain))
            {
                rdo = wrpDomain.AppDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
                Assert.IsInstanceOfType(rdo, type);
                rdo.ToString();
            }
        }

        [TestMethod]
        [DataRow(typeof(System.Data.SqlClient.SqlConnection), DisplayName = "Check with Class.")]
        //[DataRow(typeof(System.Drawing.SizeF), DisplayName = "Check with Structure.")] // Not fails for structure.
        [ExpectedException(typeof(AppDomainUnloadedException))]
        public void AppDomainWrapper_Dispose_Object(Type type)
        {
            object rdo;
            var appDomain = AppDomain.CreateDomain("TestDomain");

            using (var wrpDomain = new AppDomainWrapper(appDomain))
            {
                rdo = wrpDomain.AppDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            }
            rdo.ToString(); // Should raise AppDomainUnloadedException
            Assert.Fail("AppDomainUnloadedException was expected.");
        }

        [TestMethod]
        [ExpectedException(typeof(AppDomainUnloadedException))]
        public void AppDomainWrapper_Dispose_GetData()
        {
            var appDomain = AppDomain.CreateDomain("TestDomain");

            using (var wrpDomain = new AppDomainWrapper(appDomain))
            {
                // Do Nothing. Test dispose.
            }
            appDomain.GetData("Alehop"); // Should raise AppDomainUnloadedException
            Assert.Fail("AppDomainUnloadedException was expected.");
        }

        [TestMethod]
        public void AppDomainWrapper_IsDisposed()
        {
            var appDomain = AppDomain.CreateDomain("TestDomain");
            var wrpDomain = new AppDomainWrapper(appDomain);

            wrpDomain.Dispose();
            Assert.IsTrue(wrpDomain.IsDisposed);
        }

    }
}
