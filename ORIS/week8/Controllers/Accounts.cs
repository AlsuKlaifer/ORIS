using ORIS.week8.Attributes;

namespace ORIS.week8.Controllers
{
    [HttpController("accounts")]
    public class Accounts
    {
        List<Account> accounts = new List<Account>();

        [HttpGET("")]
        public Account GetAccount(string login)
        {
            //List<Account> accounts = new List<Account>();
            accounts.Add(new Account() { Login = "alsu", Password = "alsu" });
            accounts.Add(new Account() { Login = "andrey", Password = "1234" });

            return accounts.FirstOrDefault(t => t.Login == login);
        }

        [HttpPOST("")]
        public void PostAccount(Account account)
        {
            accounts.Add(account);
        }

        public List<Account> GetAccounts()
        {
            return accounts;
        }
    }

    public class Account
    {
        public string Login { get; set; }
        public string Password { get; set; }

    }
}

