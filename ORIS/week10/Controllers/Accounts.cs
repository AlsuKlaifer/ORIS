﻿using System.Data.SqlClient;
using ORIS.week8.Controllers;
using ORIS.week10.Attributes;
using System.Net;
using System.Net.Http.Headers;
using System.Collections.Specialized;

namespace ORIS.week10.Controllers
{
    [HttpController("accounts")]
    public class Accounts
    {
        AccountDAO accountDAO = new AccountDAO();


        [HttpGET("getById")]
        public Account GetAccountById(int id)
        {
            return accountDAO.GetById(id);
        }

        [HttpGET("getList")]
        public List<Account> GetAccounts()
        {
            return accountDAO.GetAll();
        }

        [HttpPOST("saveAccount")]
        public void SaveAccount(string login, string password)
        {
            var account = new Account(login, password);
            accountDAO.Insert(account);
        }

        [HttpPOST("postLogin")]
        public int PostLogin(string login, string password)
        {
            var account = accountDAO.GetByLogin(login);
            if (account.Password == password)
                return account.Id;
            return -1;
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

