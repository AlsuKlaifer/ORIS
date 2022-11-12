using ORIS.week8.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORIS.week9.Interfaces
{
    internal interface IRepository<T> where T : class
    {
        void Add(T t);
        void Delete(T t);
        void Update(T t);
        IEnumerable<T> All();
        T GetById(T t);
    }
}
