#if PORTABLE // EXCLUDE.
#else

using System;
using System.Collections.Generic;
using System.Data;
using qckdev.Linq;

namespace qckdev.Data
{
    static class CommandHelper
    {

        /// <remarks>
        /// <seealso href="https://stackoverflow.com/questions/7952142/how-to-resolve-system-type-to-system-data-dbtype"/>
        /// </remarks>
        readonly static Dictionary<Type, DbType> TypeMap = new Dictionary<Type, DbType>()
        {
            {typeof(byte), DbType.Byte},
            {typeof(sbyte), DbType.SByte},
            {typeof(short), DbType.Int16},
            {typeof(ushort), DbType.UInt16},
            {typeof(int), DbType.Int32},
            {typeof(uint), DbType.UInt32},
            {typeof(long), DbType.Int64},
            {typeof(ulong), DbType.UInt64},
            {typeof(float), DbType.Single},
            {typeof(double), DbType.Double},
            {typeof(decimal), DbType.Decimal},
            {typeof(bool), DbType.Boolean},
            {typeof(string), DbType.String},
            {typeof(char), DbType.StringFixedLength},
            {typeof(Guid), DbType.Guid},
            {typeof(DateTime), DbType.DateTime},
            {typeof(DateTimeOffset), DbType.DateTimeOffset},
            {typeof(byte[]), DbType.Binary},
            {typeof(byte?), DbType.Byte},
            {typeof(sbyte?), DbType.SByte},
            {typeof(short?), DbType.Int16},
            {typeof(ushort?), DbType.UInt16},
            {typeof(int?), DbType.Int32},
            {typeof(uint?), DbType.UInt32},
            {typeof(long?), DbType.Int64},
            {typeof(ulong?), DbType.UInt64},
            {typeof(float?), DbType.Single},
            {typeof(double?), DbType.Double},
            {typeof(decimal?), DbType.Decimal},
            {typeof(bool?), DbType.Boolean},
            {typeof(char?), DbType.StringFixedLength},
            {typeof(Guid?), DbType.Guid},
            {typeof(DateTime?), DbType.DateTime},
            {typeof(DateTimeOffset?), DbType.DateTimeOffset},
        };


        public static T ExecuteScalar<T>(IDbCommand command)
        {
            T rdo;
            object val;

            val = command.ExecuteScalar();
            if (val == DBNull.Value)
            {
                rdo = default;
            }
            else
            {
                var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                rdo = (T)Convert.ChangeType(val, type);
            }
            return rdo;
        }


        public static int ExecuteNonQueryAuto(IDbCommand command)
        {
            int rdo;

            using (var scope = new ConnectionScope(command.Connection))
            {
                rdo = command.ExecuteNonQuery();
            }
            return rdo;
        }

        public static object ExecuteScalarAuto(IDbCommand command)
        {
            object rdo;

            using (var scope = new ConnectionScope(command.Connection))
            {
                rdo = command.ExecuteScalar();
            }
            return rdo;
        }

        public static T ExecuteScalarAuto<T>(IDbCommand command)
        {
            T rdo;

            using (var scope = new ConnectionScope(command.Connection))
            {
                rdo = ExecuteScalar<T>(command);
            }
            return rdo;
        }

        public static IDataReader ExecuteReaderAuto(IDbCommand command)
        {
            return ExecuteReaderAuto(command, CommandBehavior.Default);
        }

        public static IDataReader ExecuteReaderAuto(IDbCommand command, CommandBehavior behavior)
        {
            IDataReader rdo;

            ConnectionHelper.OpenWithCheck(command.Connection, out ConnectionState connectionState);
            if (connectionState == ConnectionState.Closed && (behavior & CommandBehavior.CloseConnection) != CommandBehavior.CloseConnection)
            {
                behavior |= CommandBehavior.CloseConnection; // Si la conexión estaba originalmente cerrada, cerrarla otra vez tras terminar el DataReader.
            }
            rdo = command.ExecuteReader(behavior);
            return rdo;
        }

        public static DataTable ExecuteDataTableAuto(IDbCommand command)
        {
            return ExecuteDataTableAuto(command, LoadOption.PreserveChanges);
        }

        public static DataTable ExecuteDataTableAuto(IDbCommand command, LoadOption loadOption)
        {
            var table = new DataTable();

            ExecuteDataTableAuto(command, table, loadOption);
            return table;
        }

        public static void ExecuteDataTableAuto(IDbCommand command, DataTable table, LoadOption loadOption)
        {

            try
            {
                table.BeginLoadData();
                using (var reader = ExecuteReaderAuto(command))
                {
                    table.Load(reader, loadOption);
                }
            }
            finally
            {
                table.EndLoadData();
            }
        }

        /// <summary>
        /// Converts a <see cref="Type"/> to an equivalent <see cref="DbType"/> value.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to convert.</param>
        /// <returns>
        /// A <see cref="DbType"/> value that is equivalent to the specified <paramref name="type"/> 
        /// or <see cref="DbType.Object"/> if no equivalent is found.
        /// </returns>
        /// <remarks>
        /// <seealso href="https://stackoverflow.com/questions/7952142/how-to-resolve-system-type-to-system-data-dbtype"/>
        /// </remarks>
        public static DbType GetDbType(Type type)
        {
            return (TypeMap.TryGetValue(type, null) ?? DbType.Object);
        }

        public static IDbDataParameter CreateParameterWithValue<TValue>(IDbCommand command, string parameterName, TValue value)
        {
            return CreateParameterWithValue(command, parameterName, value, ParameterDirection.Input);
        }

        public static IDbDataParameter CreateParameterWithValue<TValue>(IDbCommand command, string parameterName, TValue value, ParameterDirection direction)
        {
            var p = command.CreateParameter();

            p.ParameterName = parameterName;
            p.DbType = GetDbType(typeof(TValue));
            p.Value = ((object)value ?? DBNull.Value);
            p.Direction = direction;
            return p;
        }

    }
}

#endif