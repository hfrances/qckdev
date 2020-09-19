#if PORTABLE // EXCLUDE.
#else

using System;
using System.Data;

namespace qckdev.Data
{
    static class ConnectionHelper
    {

        public static void OpenWithCheck(IDbConnection connection, out ConnectionState initialState)
        {
            initialState = connection.State;
            if (initialState == ConnectionState.Closed)
                connection.Open();
        }

        public static void CloseWithCheck(IDbConnection connection, ConnectionState initialState)
        {
            if (initialState == ConnectionState.Closed && connection.State != ConnectionState.Closed)
                connection.Close();
        }

        public static T Clone<T>(T connection) where T: IDbConnection
        {
            return (T)((ICloneable)connection).Clone();
        }
    }
}

#endif