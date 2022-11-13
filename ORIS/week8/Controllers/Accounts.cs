using System.Data.SqlClient;
using Microsoft.VisualBasic;
using ORIS.week8.Attributes;

namespace ORIS.week8.Controllers
{
    [HttpController("accounts")]
    public class Accounts
    {
        [HttpGET("getById")]
        public Account GetAccountById(int id)
        {
            Account account = null;

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = "SELECT * FROM dbo.Accounts";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        if (reader.GetInt32(0) == id)
                        {
                            account = new Account
                            (
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2)
                            );
                        }
                    }
                }
                reader.Close();
            }
            return account;
        }

        [HttpGET("getList")]
        public List<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = "SELECT * FROM dbo.Accounts";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        accounts.Add(new Account
                        (
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2)
                        ));
                    }
                }
                reader.Close();
            }
            return accounts;
        }

        [HttpPOST("saveAccount")]
        public void SaveAccount(string login, string password)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand();

                command.CommandText = "INSERT INTO dbo.Accounts (Login, Password) VALUES (@login, @password)"; 
                command.Connection = connection;
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                command.ExecuteScalar();
            }
        }

        //[HttpGET("saveAccount")]
        //public void SaveAccount(string login, string password)
        //{
        //    Console.WriteLine("Test:");
        //    Console.WriteLine("Login: " + login + " Password: " + password);
        //}
    }

    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public Account(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }
    }
}

