namespace ReliableDatabaseClient
{
    using System;
    using System.Data;

    public interface IDbAccess
    {
        IDbConnection GetDbConnection();

        T Get<T>(Func<IDbConnection, T> query);

        void Execute(Action<IDbConnection> query);
    }
}