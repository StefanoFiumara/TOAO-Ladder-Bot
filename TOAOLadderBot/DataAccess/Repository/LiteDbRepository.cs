using System;
using System.Linq;
using LiteDB;
using TOAOLadderBot.DataAccess.Models;

namespace TOAOLadderBot.DataAccess.Repository
{
    public sealed class LiteDbRepository<T> : IRepository<T> 
        where T : class, IDbModel
    {
        private readonly LiteDbContext _context;
        private readonly LiteDatabase _database;

        public ILiteQueryable<T> Query => _database.GetCollection<T>().Query();

        public LiteDbRepository(LiteDbContext context, LiteDatabase database)
        {
            _context = context;
            _database = database;

        }
        
        public T Create(T entity)
        {
            // TODO: check if this is necessary or if LiteDB will generate it for us
            // entity.Id = ObjectId.NewObjectId();
            _context.AddCommand(() =>
            {
                _database.GetCollection<T>().Insert(entity);
            });
            
            return entity;
        }

        public bool Update(T entity)
        {
            return _database.GetCollection<T>().Update(entity);
        }

        public bool Delete(T entity)
        {
            return _database.GetCollection<T>().Delete(entity.Id);
        }
    }
}