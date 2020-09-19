#if PORTABLE // EXCLUDE.
#else

using System;
using System.Data;

namespace qckdev.Data
{

    /// <summary>
    /// Defines the extension methods to the <see cref="System.Data"/> namespace.
    /// </summary>
    public static class DataExtensions
    {

        /// <summary>
        /// Creates a new connection that is a copy of the current instance.
        /// </summary>
        /// <param name="connection">The connection to clone.</param>
        /// <returns>A new connection that is a copy of this instance.</returns>
        public static IDbConnection Clone<T>(this T connection) where T : IDbConnection
        {
            return ConnectionHelper.Clone(connection);
        }

        /// <summary>
        /// Executes an SQL statement against the Connection object of a .NET Framework data provider, and returns the number of rows affected.
        /// If the connection is closed, it is automatically opened and closed. Otherwise the connection status does not change.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="System.InvalidOperationException">The connection does not exist. -or- The connection is not open.</exception>
        public static int ExecuteNonQueryAuto(this IDbCommand command)
        {
            return CommandHelper.ExecuteNonQueryAuto(command);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The returned value type.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first column of the first row in the resultset converted to <typeparamref name="T"/> type. 
        /// When the result is <see cref="DBNull.Value"/>, it is converted to null.
        /// </returns>
        public static T ExecuteScalar<T>(this IDbCommand command)
        {
            return CommandHelper.ExecuteScalar<T>(command);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
        /// If the connection is closed, it is automatically opened and closed. Otherwise the connection status does not change.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first column of the first row in the resultset.</returns>
        public static object ExecuteScalarAuto(this IDbCommand command)
        {
            return CommandHelper.ExecuteScalarAuto(command);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
        /// If the connection is closed, it is automatically opened and closed. Otherwise the connection status does not change.
        /// </summary>
        /// <typeparam name="T">The returned value type.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first column of the first row in the resultset converted to <typeparamref name="T"/> type. 
        /// When the result is <see cref="DBNull.Value"/>, it is converted to null.
        /// </returns>
        public static T ExecuteScalarAuto<T>(this IDbCommand command)
        {
            return CommandHelper.ExecuteScalarAuto<T>(command);
        }

        /// <summary>
        /// Executes the <see cref="System.Data.IDbCommand.CommandText"/> against the <see cref="System.Data.IDbCommand.Connection"/> 
        /// and builds an <see cref="System.Data.IDataReader"/>.
        /// If the connection is closed, it is automatically opened and closed. Otherwise the connection status does not change.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>An <see cref="IDataReader"/> object.</returns>
        public static IDataReader ExecuteReaderAuto(this IDbCommand command)
        {
            return CommandHelper.ExecuteReaderAuto(command);
        }

        /// <summary>
        /// Executes the <see cref="System.Data.IDbCommand.CommandText"/> against the <see cref="System.Data.IDbCommand.Connection"/>, 
        /// and builds an <see cref="System.Data.IDataReader"/> using one of the <see cref="System.Data.CommandBehavior"/> values.
        /// If the connection is closed, it is automatically opened and closed. Otherwise the connection status does not change.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">One of the System.Data.CommandBehavior values.</param>
        /// <returns>An <see cref="IDataReader"/> object.</returns>
        public static IDataReader ExecuteReaderAuto(this IDbCommand command, CommandBehavior behavior)
        {
            return CommandHelper.ExecuteReaderAuto(command, behavior);
        }

        /// <summary>
        /// Returns a <see cref="System.Data.DataTable"/> with values from a data source using the supplied <see cref="System.Data.IDbCommand"/>. 
        /// </summary>
        /// <param name="command">The <see cref="System.Data.IDbCommand"/> to execute.</param>
        public static DataTable ExecuteDataTableAuto(this IDbCommand command)
        {
            return CommandHelper.ExecuteDataTableAuto(command);
        }

        /// <summary>
        /// Fills a <see cref="System.Data.DataTable"/> with values from a data source using the supplied <see cref="System.Data.IDbCommand"/>. 
        /// If the <see cref="System.Data.DataTable"/> already contains rows, the incoming data from the data source is merged with the existing rows according to the value of the <paramref name="loadOption"/> parameter.
        /// </summary>
        /// <param name="command">The <see cref="System.Data.IDbCommand"/> to execute.</param>
        /// <param name="table">The <see cref="System.Data.DataTable"/> to fill.</param>
        /// <param name="loadOption">
        /// A value from the System.Data.LoadOption enumeration that indicates how rows already in the <see cref="System.Data.DataTable"/> are combined with incoming rows that share the same primary key.
        /// </param>
        public static void ExecuteDataTableAuto(this IDbCommand command, DataTable table, LoadOption loadOption)
        {
            CommandHelper.ExecuteDataTableAuto(command, table, loadOption);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="System.Data.IDbDataParameter"/> object.
        /// </summary>
        /// <typeparam name="TValue">Value type of the parameter.</typeparam>
        /// <param name="command"></param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value to be added. If the value is null, it sends a <see cref="DBNull.Value"/>.</param>
        /// <returns>A <see cref="System.Data.IDbDataParameter"/> object.</returns>
        public static IDbDataParameter CreateParameterWithValue<TValue>(this IDbCommand command, string parameterName, TValue value)
        {
            return CommandHelper.CreateParameterWithValue(command, parameterName, value);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="System.Data.IDbDataParameter"/> object.
        /// </summary>
        /// <typeparam name="TValue">Value type of the parameter.</typeparam>
        /// <param name="command"></param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value to be added. If the value is null, it sends a <see cref="DBNull.Value"/>.</param>
        /// <param name="direction">One of the <see cref="System.Data.ParameterDirection"/> values.</param>
        /// <returns>A <see cref="System.Data.IDbDataParameter"/> object.</returns>
        public static IDbDataParameter CreateParameterWithValue<TValue>(this IDbCommand command, string parameterName, TValue value, ParameterDirection direction)
        {
            return CommandHelper.CreateParameterWithValue(command, parameterName, value, direction);
        }

    }
}

#endif