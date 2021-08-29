using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.DataAccess.Repository
{
    public interface IRepository<TEntity>
        where TEntity : class, ILiteDbDocument
    {
        ILiteQueryable<TEntity> Query { get; }

        TEntity Create(TEntity entity);
        
        int CreateMany(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);

        TEntity Upsert(TEntity entity);

        TEntity Delete(TEntity entity);
        
    }
}