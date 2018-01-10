namespace ReliableDatabaseClient.Repositories
{
    using Dapper;
    using ReliableDatabaseClient.Models;
    using System.Collections.Generic;
    using System.Data;
    
    public class UsersRepository : IRepository<Users>
    {
        private readonly IDbAccess dbAccess;

        public UsersRepository(IDbAccess dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public IEnumerable<Users> GetAll()
        {
            var query = "SELECT * FROM Users";

            return dbAccess.Get<IEnumerable<Users>>(db =>
                db.Query<Users>(query, CommandType.Text));
        }

        public void Save(Users data)
        {
            var query = $"INSERT INTO Users([UserId], [Name]) VALUES('{data.UserId}', '{data.Name}')";

            dbAccess.Execute(db =>
                db.Execute(query, CommandType.Text));
        }
    }
}