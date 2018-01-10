namespace ReliableDatabaseClient
{
    using ReliableDatabaseClient.Policies;
    using ReliableDatabaseClient.Settings;
    using System;
    using System.Data;

    public class DbAccess : IDbAccess
    {
        private readonly IRetryPolicy retryPolicy;

        public DbAccess(IRetryPolicy retryPolicy)
        {
            this.retryPolicy = retryPolicy;
        }

        public IDbConnection GetDbConnection()
        {
            return new ReliableDbConnection(Connection.ConnectionString, retryPolicy);
        }

        public T Get<T>(Func<IDbConnection, T> query)
        {
            using (var db = GetDbConnection())
            {
                return query.Invoke(db);
            }
        }

        public void Execute(Action<IDbConnection> query)
        {
            using (var db = GetDbConnection())
            {
                query.Invoke(db);
            }
        }
    }
}