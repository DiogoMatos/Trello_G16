using System.Collections.Generic;

namespace Etapa2.Repository
{
    interface IRepository<T, K>
    {
        IEnumerable<T> GetAll();
        T GetById(K id);
        K Add(T elem);
    }
}
