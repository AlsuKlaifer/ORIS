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
            orm.Insert<Account>(account);
        }
        
        public IEnumerable<Account> All()
        {
            return orm.Select<Account>().ToList();
        }

        public void Delete(Account account)
        {
            orm.Delete<Account>(account);  
        }

        public void Update(Account account)
        {
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", account.Password)
                .ExecuteNonQuery("UPDATE dbo.Accounts SET Password = @password WHERE Login = @login");
        }

        public Account GetById(Account account)
        {
            return orm.AddParameter("@id", account.Id).ExecuteQuery<Account>("SELECT * FROM dbo.Accounts WHERE Id = @id").FirstOrDefault();
        }
    }
}
