using System;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace derpirc.Data
{
    public class SqlRepository<T> : IRepository<T> where T : class, IBaseModel
    {
        public SqlRepository(DataContext context)
        {
            _objectSet = context.GetTable<T>();
        }

        public IQueryable<T> FindAll()
        {
            return _objectSet;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate);
        }

        public T FindById(int id)
        {
            return _objectSet.SingleOrDefault(o => o.Id == id);
        }

        public void Add(T newEntity)
        {
            _objectSet.InsertOnSubmit(newEntity);
        }

        public void Remove(T entity)
        {
            _objectSet.DeleteOnSubmit(entity);
        }

        protected ITable<T> _objectSet;
    }
}