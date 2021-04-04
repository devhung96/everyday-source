using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Services
{
    public interface IService<T>
    {
        T GetById(string id);
        void Delete(T entity);
    }
}
