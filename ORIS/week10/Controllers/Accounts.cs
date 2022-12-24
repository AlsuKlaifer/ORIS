using System.Data.SqlClient;
using ORIS.week8.Controllers;
using ORIS.week10.Attributes;
using System.Net;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using System.Text.Json;

namespace ORIS.week10.Controllers
{
    [HttpController("accounts")]
    public class Accounts
    {
        AccountDAO accountDAO = new AccountDAO();
        HttpListenerContext _httpContent;

        public Accounts(HttpListenerContext httpContent)
        {
            _httpContent = httpContent;
        }

        [HttpGET("getById")]
        public Account GetAccountById(int id)
        {
            return accountDAO.GetById(id);
        }

        [HttpGET("getList")]
        public HttpResponseMessage GetAccounts()
        {
            if(_httpContent.Request.Cookies["SessionId_IsAuthorize"] == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            string cookie_IsAuthorize = _httpContent.Request.Cookies["SessionId_IsAuthorize"].Value;
            string cookie_Id = _httpContent.Request.Cookies["SessionId_Id"].Value;
            if (bool.Parse(cookie_IsAuthorize))
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(String.Join(", ", accountDAO.GetAll()));
                return responseMessage;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        [HttpPOST("saveAccount")]
        public void SaveAccount(string login, string password)
        {
            var account = new Account(login, password);
            accountDAO.Insert(account);
        }

        [HttpPOST("postLogin")]
        public bool PostLogin(string login, string password)
        {
            var account = accountDAO.GetByLogin(login);
            if (account.Password == password)
            {
                var cookie = new Cookie("SessionId_IsAuthorize", "true");
                var cookie2 = new Cookie("SessionId_Id", $"{account.Id}");
                _httpContent.Response.SetCookie(cookie);
                _httpContent.Response.SetCookie(cookie2);
                return true;
            }
            return false;
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public Account(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public Account(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }

        public Account() { }
    }
}

