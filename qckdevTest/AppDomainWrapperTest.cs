using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qckdev;

namespace qckdevTest
{

    [TestClass]
    public class AppDomainWrapperTest
    {

        [TestMethod]
        public void AppDomainWrapper_InstanceAndUnwrap()
        {
            bool rdo;
            var appDomain = AppDomain.CreateDomain("TestDomain");
            var boolType = typeof(bool);
            var assemblyName = boolType.Assembly.FullName;
            var boolTypeName = boolType.FullName;

            using (var wrpDomain = new AppDomainWrapper(appDomain))
            {
                rdo = (bool)wrpDomain.AppDomain.CreateInstanceAndUnwrap(assemblyName, boolTypeName);
                rdo = true;
            }

            Assert.IsTrue(rdo);
        }

        [TestMethod]
        [ExpectedException(typeof(AppDomainUnloadedException))]
        public void AppDomainWrapper_Dispose()
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
