using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TOAOLadderBot.DataAccess.Models;

namespace TOAOLadderBot.DataAccess.Repository
{
    public sealed class LiteDbRepository<TEntity> : IRepository<TEntity> 
        where TEntity : class, ILiteDbDocument
    {
        private readonly LiteDbContext _context;
        private readonly LiteDatabase _database;

        public ILiteQueryable<TEntity> Query => _database.GetCollection<TEntity>().Query();

        public LiteDbRepository(LiteDbContext context, LiteDatabase database)
        {
            _context = context;
            _database = database;
        }
        
        public TEntity Create(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _database.GetCollection<TEntity>().Insert(entity);
            });
            
            return entity;
        }

        public int CreateMany(IEnumerable<TEntity> entities)
        {
            _context.AddCommand(() =>
            {
                _database.GetCollection<TEntity>().InsertBulk(entities);
            });
            
            return entities.Count();
        }

        public TEntity Update(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _database.GetCollection<TEntity>().Update(entity);
            });

            return entity;
        }

        public TEntity Upsert(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _database.GetCollection<TEntity>().Upsert(entity);
            });

            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _database.GetCollection<TEntity>().Delete(entity.Id);
            });

            return entity;
        }
    }
}