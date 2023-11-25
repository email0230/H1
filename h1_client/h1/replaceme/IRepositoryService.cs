using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.replaceme
{
    public interface IRepositoryService<T> where T : class
    {
        void Create(T entity);
        void Read(T entity);
        void Update(T entity);
        void Delete(T entity);
        //they used to be returning a "serviceresult" object, check out why that is
    }
}
