using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ORIS.week8.Controllers;
using ORIS.week9.Attributes;

namespace ORIS.week9.Controllers
{
    public static class SQLCommands
    {
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

        public static List<T> Select<T>()
        {
            var orm = new MyORM(connectionString);
            return orm.ExecuteQuery<T>("SELECT * FROM [dbo].[Table]").ToList();
        }

        public static void Insert<T>(Account account)
        {
            var orm = new MyORM(connectionString);
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", account.Password)
                .ExecuteNonQuery("INSERT INTO [dbo].[Table] (Login, Password) VALUES (@login, @password)");
        }

        public static void Delete(string login)
        {
            var orm = new MyORM(connectionString);
            orm.AddParameter("@login", login)
                .ExecuteNonQuery("DELETE FROM [dbo].[Table] WHERE Login = @login");
        }

        public static void Update(Account account, string newPassword)
        {
            var orm = new MyORM(connectionString);
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", newPassword)
                .ExecuteNonQuery("UPDATE [dbo].[Table] SET Password = @password WHERE Login = @login");
        }
    }
}
