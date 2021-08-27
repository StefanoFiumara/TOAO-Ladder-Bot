using System;
using System.Linq;

namespace TOAOLadderBot.DataAccess
{
    public sealed class LiteDbRepository<T> : IRepository<T> where T : class
    {
        public IQueryable<T> Query { get; }
        public T Create(T entity)
        {
            throw new NotImplementedException();
        }

        public T Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }
    }
}