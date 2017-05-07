using System;
using System.Data;

namespace StkCommon.TestUtils.MockDataBase
{
    /// <summary>
    /// IDbConnection
    /// </summary>
    public partial class MockDb : IDbConnection
    {
        private ConnectionState _state;

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            throw new NotSupportedException();
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            throw new NotSupportedException();
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException();
        }

        void IDbConnection.Close()
        {
            _state = ConnectionState.Closed;
        }

        string IDbConnection.ConnectionString { get; set; }

        int IDbConnection.ConnectionTimeout
        {
            get { throw new NotSupportedException(); }
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            return new MockCommand(this);
        }

        string IDbConnection.Database
        {
            get { throw new NotSupportedException(); }
        }

        void IDbConnection.Open()
        {
            _state = ConnectionState.Open;
        }

        ConnectionState IDbConnection.State
        {
            get { return _state; }
        }

        void IDisposable.Dispose()
        {
        }
    }
}