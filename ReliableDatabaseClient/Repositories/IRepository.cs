namespace ReliableDatabaseClient.Repositories
{
    using System.Collections.Generic;

    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        void Save(T data);
    }
}