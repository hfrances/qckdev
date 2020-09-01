using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qckdev.Linq;
using qckdev.Data;
using System.Data;

namespace qckdevTest.Data
{
    [TestClass]
    public class DataHelperTest
    {

        const string CONNSTRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        #region command auto

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ExecuteNonQueryAutoTest(bool openBeforeStart)
        {

            using (var conn = new System.Data.SqlClient.SqlConnection(CONNSTRING))
            {
                if (openBeforeStart)
                    conn.Open();

                using (var comm = conn.CreateCommand())
                {
                    int rdo;

                    comm.CommandText = "sp_who2";
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    rdo = comm.ExecuteNonQueryAuto();
                    Assert.AreNotEqual(0, rdo);
                }

                if (openBeforeStart)
                    Assert.AreEqual(System.Data.ConnectionState.Open, conn.State);
                else
                    Assert.AreEqual(System.Data.ConnectionState.Closed, conn.State);
            }
        }

        [TestMethod]
        [DataRow(false, "hola mundo", "hola mundo")]
        [DataRow(false, "", "")]
        [DataRow(false, Assert2.DBNullCONST, null)]
        [DataRow(true, Assert2.DBNullCONST, null)]
        public void ExecuteScalarAutoTest(bool openBeforeStart, object expected, string value)
        {

            using (var conn = new System.Data.SqlClient.SqlConnection(CONNSTRING))
            {
                if (openBeforeStart)
                    conn.Open();

                using (var comm = conn.CreateCommand())
                {
                    object rdo;

                    comm.CommandText = $"SELECT @param";
                    comm.Parameters.AddWithValue("@param", (object)value ?? DBNull.Value);
                    rdo = comm.ExecuteScalarAuto();
                    Assert2.AreEqualDBNull(expected, rdo);
                }

                if (openBeforeStart)
                    Assert.AreEqual(System.Data.ConnectionState.Open, conn.State);
                else
                    Assert.AreEqual(System.Data.ConnectionState.Closed, conn.State);
            }
        }

        [TestMethod]
        [DataRow(false, "hola mundo", "hola mundo")]
        [DataRow(false, "", "")]
        [DataRow(false, null, null)]
        [DataRow(true, null, null)]
        public void ExecuteScalarAutoTTest(bool openBeforeStart, object expected, string value)
        {

            using (var conn = new System.Data.SqlClient.SqlConnection(CONNSTRING))
            {
                if (openBeforeStart)
                    conn.Open();

                using (var comm = conn.CreateCommand())
                {
                    string rdo;

                    comm.CommandText = $"SELECT @param";
                    comm.Parameters.AddWithValue("@param", (object)value ?? DBNull.Value);
                    rdo = comm.ExecuteScalarAuto<string>();
                    Assert2.AreEqualDBNull(expected, rdo);
                }

                if (openBeforeStart)
                    Assert.AreEqual(System.Data.ConnectionState.Open, conn.State);
                else
                    Assert.AreEqual(System.Data.ConnectionState.Closed, conn.State);
            }
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ExecuteReaderAutoTest(bool openBeforeStart)
        {

            using (var conn = new System.Data.SqlClient.SqlConnection(CONNSTRING))
            {
                if (openBeforeStart)
                    conn.Open();

                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = $"SELECT * FROM syslanguages";
                    using (var reader = comm.ExecuteReaderAuto())
                    {
                        while (reader.Read())
                        {
                            reader.ToString();
                        }
                    }
                }

                if (openBeforeStart)
                    Assert.AreEqual(System.Data.ConnectionState.Open, conn.State);
                else
                    Assert.AreEqual(System.Data.ConnectionState.Closed, conn.State);
            }
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ExecuteDataTableAutoTest(bool openBeforeStart)
        {

            using (var conn = new System.Data.SqlClient.SqlConnection(CONNSTRING))
            {
                if (openBeforeStart)
                    conn.Open();

                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = $"SELECT * FROM syslanguages";
                    using (var dataTable = comm.ExecuteDataTableAuto())
                    {
                        Assert.IsTrue(dataTable.Columns.OfType<DataColumn>().Any(), "No columns were loaded.");
                        Assert.IsTrue(dataTable.Columns.OfType<DataColumn>().Any(), "No rows were loaded");
                    }
                }

                if (openBeforeStart)
                    Assert.AreEqual(System.Data.ConnectionState.Open, conn.State);
                else
                    Assert.AreEqual(System.Data.ConnectionState.Closed, conn.State);
            }
        }

        #endregion


        #region create parameter

        [TestMethod]
        [DataRow(true, "hello world", "hello world")]
        [DataRow(false, "hello world", "hello world")]
        [DataRow(true, null, null)]
        [DataRow(false, null, Assert2.DBNullCONST)]
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
        [DataRow(false, null, Assert2.DBNullCONST)]
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
                    Assert2.AreEqualDBNull(expectedResult, rdo);
                }
            }
        }


        #endregion

    }
}
