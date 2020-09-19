#if PORTABLE // EXCLUDE.
#else

using System;
using System.Data;

namespace qckdev.Data
{

    sealed class ConnectionScope : IDisposable
    {

        #region ctor

        public ConnectionScope(IDbConnection connection)
            : this(connection, createTransaction: false) { }

        public ConnectionScope(IDbConnection connection, bool createTransaction)
            : this(connection, createTransaction, isolationLevel: null) { }

        public ConnectionScope(IDbConnection connection, IsolationLevel isolationLevel)
            : this(connection, createTransaction: true, isolationLevel: IsolationLevel.Unspecified) { }

        private ConnectionScope(IDbConnection connection, bool createTransaction, IsolationLevel? isolationLevel)
        {
            this.Connection = connection;

            ConnectionHelper.OpenWithCheck(connection, out ConnectionState initialState);
            this.InitialState = initialState;

            if (createTransaction)
            {
                if (isolationLevel == null)
                    this.Transaction = connection.BeginTransaction();
                else
                    this.Transaction = connection.BeginTransaction(isolationLevel.Value);
            }
        }

        #endregion

        #region properties

        public IDbConnection Connection { get; }
        public ConnectionState InitialState { get; }
        public IDbTransaction Transaction { get; }

        #endregion

        #region methods

        public void Dispose()
        {
            Transaction?.Dispose();
            ConnectionHelper.CloseWithCheck(this.Connection, this.InitialState);
        }

        #endregion

    }
}

#endif