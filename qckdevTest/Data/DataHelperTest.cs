using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qckdev.Linq;
using qckdev.Data;

namespace qckdevTest.Data
{
    [TestClass]
    public class DataHelperTest
    {

        const string CONNSTRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        const string DBNullCONST = "1B544BD7-608D-438F-B6DC-F3A1B538A898";

        [TestMethod]
        [DataRow(true, "hello world", "hello world")]
        [DataRow(false, "hello world", "hello world")]
        [DataRow(true, null, null)]
        [DataRow(false, null, DBNullCONST)]
        public void CreateParameterWithValueTest_String(bool castResult, string parameterValue, object expectedResult)
        {
            CreateParameterWithValueTest<string>(castResult, parameterValue, expectedResult);
        }

        [TestMethod]
        [DataRow(true, 1, 1)]
        [DataRow(false, 1, 1)]
        [DataRow(true, 0, 0)]
        [DataRow(false, 0, 0)]
        public void CreateParameterWithValueTest_Integer(bool castResult, int parameterValue, object expectedResult)
        {
            CreateParameterWithValueTest<int>(castResult, parameterValue, expectedResult);
        }

        [TestMethod]
        [DataRow(true, 1, 1)]
        [DataRow(false, 1, 1)]
        [DataRow(true, 0, 0)]
        [DataRow(false, 0, 0)]
        [DataRow(true, null, null)]
        [DataRow(false, null, DBNullCONST)]
        public void CreateParameterWithValueTest_IntegerNullable(bool castResult, int? parameterValue, object expectedResult)
        {
            CreateParameterWithValueTest<int?>(castResult, parameterValue, expectedResult);
        }

        private void CreateParameterWithValueTest<T>(bool castResult, T parameterValue, object expectedResult)
        {

            using (var conn = new System.Data.SqlClient.SqlConnection(CONNSTRING))
            {
                using (var comm = conn.CreateCommand())
                {
                    object rdo = null;

                    comm.CommandText = "SELECT @param";
                    comm.Parameters.Add(comm.CreateParameterWithValue("@param", parameterValue));

                    for (int i = 0; i < 1000; i++) // Repetir varias veces para comprobar eficiencia.
                    {
                        if (castResult)
                            rdo = comm.ExecuteScalarAuto<T>();
                        else
                            rdo = comm.ExecuteScalarAuto();
                    }

                    if (object.Equals(expectedResult, DBNullCONST))
                        Assert.AreEqual(DBNull.Value, rdo);
                    else
                        Assert.AreEqual(parameterValue, rdo);
                }
            }
        }

    }
}
