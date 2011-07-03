using System;
using System.Linq;
using System.Linq.Expressions;

namespace derpirc.Data
{
    // Mostly from http://msdn.microsoft.com/en-us/ff714955.aspx
    public interface IRepository<T> where T: class
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        T FindById(int id);
        void Add(T newEntity);
        void Remove(T entity);
    }
}
