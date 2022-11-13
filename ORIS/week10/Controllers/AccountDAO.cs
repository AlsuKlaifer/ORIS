using ORIS.week10.Interfaces;
using ORIS.week10.Controllers;

namespace ORIS.week10.Controllers
{
    public class AccountDAO : IDAO<Account>
    {
        const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        MyORM orm = new MyORM(connectionString);

        public void Delete(Account account)
        {
            orm.Delete<Account>(account);
        }

        public List<Account> GetAll()
        { 
            return orm.Select<Account>().ToList();
        }

        public Account GetById(int id)
        {
            return orm.AddParameter("@id", id).ExecuteQuery<Account>("SELECT * FROM dbo.Accounts WHERE Id = @id").FirstOrDefault();
        }

        public Account GetByLogin(string login)
        {
            return orm.AddParameter("@login", login).ExecuteQuery<Account>("SELECT * FROM dbo.Accounts WHERE Login = @login").FirstOrDefault();
        }

        public void Insert(Account account)
        {
            orm.Insert<Account>(account);
        }

        public void Update(Account account, string newPassword)
        {
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", newPassword)
                .ExecuteNonQuery("UPDATE dbo.Accounts SET Password = @password WHERE Login = @login");
        }
    }
}
