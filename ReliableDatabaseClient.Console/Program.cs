namespace ReliableDatabaseClient.Console
{
    using ReliableDatabaseClient.Models;
    using ReliableDatabaseClient.Policies;
    using ReliableDatabaseClient.Repositories;
    using ReliableDatabaseClient.Settings;
    using System;
    using System.Collections.Generic;
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter UserId:");
            var userId = Console.ReadLine();

            Console.WriteLine("Enter Name:");
            var name = Console.ReadLine();

            var retryCount = 3;
            var retryIntervalInMilliseconds = 1000;
            var sqlExceptionNumbers = new[] { 53, -2 };

            /* 53 = 
             * An error has occurred while establishing a connection to the server. 
             * When connecting to SQL Server, this failure may be caused by the fact that 
             * under the default settings SQL Server does not allow remote connections. 
             * (provider: Named Pipes Provider, error: 40 - Could not open a connection to SQL Server ) 
             * (.Net SqlClient Data Provider).             * 
             */

            /* -2 = 
             * Timeout expired. 
             * The timeout period elapsed prior to completion of the operation or the server is not responding. 
             * (Microsoft SQL Server, Error: -2).             * 
             */


            var dbAccess = new DbAccess(new DbRetryPolicy(retryCount, retryIntervalInMilliseconds, sqlExceptionNumbers));
            var userRepository = new UsersRepository(dbAccess);

            Connection.ConnectionString = "Integrated Security=SSPI;Initial Catalog=Friends;Data Source=.\\sqlexpress;";

            userRepository.Save(new Users { UserId = userId, Name = name });

            var users = userRepository.GetAll();
            DisplayUsers(users);

            Console.WriteLine("Want to see retries for Get? (Y/N)");
            var option = Console.ReadLine();

            if (option.ToLower().Equals("y"))
            {
                try
                {
                    Connection.ConnectionString = "Integrated Security=SSPI;Initial Catalog=UnknownDb;Data Source=UnknownSource;";
                    users = userRepository.GetAll();
                    DisplayUsers(users);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                }
            }

            Console.WriteLine("Press enter or exit");
            Console.ReadLine();
        }

        private static void DisplayUsers(IEnumerable<Users> users)
        {
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id} - UserId: {user.UserId} - Name: {user.Name}");
            }
        }
    }
}