using ORIS.week9.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ORIS.week9.Controllers
{
    internal class AccountRepository : IRepository<Account>
    {
        const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        MyORM orm = new MyORM(connectionString);
        public void Add(Account account)
        {
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", account.Password)
                .ExecuteNonQuery("INSERT INTO [dbo].[Table] (Login, Password) VALUES (@login, @password)");
        }
        
        public IEnumerable<Account> All()
        {
            return orm.ExecuteQuery<Account>("SELECT * FROM [dbo].[Table]").ToList();
        }

        public void Delete(Account account)
        {
            orm.AddParameter("@id", account.Id)
                .ExecuteNonQuery("DELETE FROM [dbo].[Table] WHERE Id = @id");
        }

        public void Update(Account account)
        {
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", account.Password)
                .ExecuteNonQuery("UPDATE [dbo].[Table] SET Password = @password WHERE Login = @login");
        }

        public Account GetById(Account account)
        {
            return orm.AddParameter("@id", account.Id).ExecuteQuery<Account>("SELECT * FROM [dbo].[Table] WHERE Id = @id").FirstOrDefault();
        }
    }
}
