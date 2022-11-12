using ORIS.week8.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORIS.week9.Interfaces
{
    public interface IDAO<Account>
    {
        void Insert(Account account);

        void Update(Account account, string newPassword);

        void Delete(Account account);

        List<Account> GetAll();

        Account GetById(int id);
    }
}
