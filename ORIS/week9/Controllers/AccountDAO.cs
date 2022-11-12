using ORIS.week9.Interfaces;
using ORIS.week9.Controllers;

namespace ORIS.week9.Controllers
{
    public class AccountDAO : IDAO<Account>
    {
        const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        MyORM orm = new MyORM(connectionString);

        public void Delete(Account account)
        {
            orm.AddParameter("@id", account.Id)
                .ExecuteNonQuery("DELETE FROM [dbo].[Table] WHERE Id = @id");
        }

        public List<Account> GetAll()
        {
            return orm.ExecuteQuery<Account>("SELECT * FROM [dbo].[Table]").ToList();
        }

        public Account GetById(int id)
        {
            return orm.AddParameter("@id", id).ExecuteQuery<Account>("SELECT * FROM [dbo].[Table] WHERE Id = @id").FirstOrDefault();
        }

        public void Insert(Account account)
        {
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", account.Password)
                .ExecuteNonQuery("INSERT INTO [dbo].[Table] (Login, Password) VALUES (@login, @password)");
        }

        public void Update(Account account, string newPassword)
        {
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", newPassword)
                .ExecuteNonQuery("UPDATE [dbo].[Table] SET Password = @password WHERE Login = @login");
        }
    }
}
