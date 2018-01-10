namespace ReliableDatabaseClient
{
    using ReliableDatabaseClient.Policies;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    public class ReliableDbConnection : DbConnection
    {
        private readonly SqlConnection underlyingConnection;
        private readonly IRetryPolicy retryPolicy;
        private string connectionString;

        public ReliableDbConnection(string connectionString, IRetryPolicy retryPolicy)
        {
            this.connectionString = connectionString;
            this.retryPolicy = retryPolicy;
            this.underlyingConnection = new SqlConnection(connectionString);
        }

        public override string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                this.connectionString = value;
                this.underlyingConnection.ConnectionString = value;
            }
        }

        public override string Database => this.underlyingConnection.Database;

        public override string DataSource => this.underlyingConnection.DataSource;

        public override string ServerVersion => this.underlyingConnection.ServerVersion;

        public override ConnectionState State => this.underlyingConnection.State;

        public override void ChangeDatabase(string databaseName)
        {
            this.underlyingConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            this.underlyingConnection.Close();
        }

        public override void Open()
        {
            retryPolicy.Execute(() =>
            {
                if (this.underlyingConnection.State != ConnectionState.Open)
                {
                    this.underlyingConnection.Open();
                }
            });
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return this.underlyingConnection.BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return underlyingConnection.CreateCommand();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (underlyingConnection.State == ConnectionState.Open)
                {
                    underlyingConnection.Close();
                }

                underlyingConnection.Dispose();
            }

            GC.SuppressFinalize(this);
        }

    }
}