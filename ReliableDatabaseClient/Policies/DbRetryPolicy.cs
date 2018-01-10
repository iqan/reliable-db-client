namespace ReliableDatabaseClient.Policies
{
    using Polly;
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class DbRetryPolicy : IRetryPolicy
    {
        private readonly Policy _retryPolicyAsync;
        private readonly Policy _retryPolicy;

        public DbRetryPolicy(int retryCount, double retryIntervalInMilliseconds, int[] exceptionNumbers)
        {
            _retryPolicyAsync = Policy
                .Handle<SqlException>(exception => exceptionNumbers.Contains(exception.Number))
                .WaitAndRetryAsync(
                    retryCount: retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(retryIntervalInMilliseconds),
                    onRetryAsync: (ex, time) => Task.Run(() => Console.WriteLine("Retrying"))
                );

            _retryPolicy = Policy
                .Handle<SqlException>(exception => exceptionNumbers.Contains(exception.Number))
                .WaitAndRetry(
                    retryCount: retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(retryIntervalInMilliseconds),
                    onRetry: (ex, time) => Task.Run(() => Console.WriteLine("Retrying"))
                );
        }

        public void Execute(Action operation)
        {
            _retryPolicy.Execute(operation.Invoke);
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return _retryPolicy.Execute(() => operation.Invoke());
        }

        public async Task Execute(Func<Task> operation, CancellationToken cancellationToken)
        {
            await _retryPolicyAsync.ExecuteAsync(operation.Invoke);
        }

        public async Task<TResult> Execute<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            return await _retryPolicyAsync.ExecuteAsync(operation.Invoke);
        }
    }
}