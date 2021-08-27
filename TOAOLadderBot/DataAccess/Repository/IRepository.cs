using System.Linq;
using LiteDB;
using TOAOLadderBot.DataAccess.Models;

namespace TOAOLadderBot.DataAccess.Repository
{
    public interface IRepository<TEntity>
        where TEntity : class, ILiteDbDocument
    {
        ILiteQueryable<TEntity> Query { get; }

        TEntity Create(TEntity entity);

        bool Update(TEntity entity);

        bool Delete(TEntity entity);
        
    }
}