using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.DataAccess.Repository
{
    public sealed class LiteDbRepository<TEntity> : IRepository<TEntity> 
        where TEntity : class, ILiteDbDocument
    {
        private readonly LiteDbContext _context;

        public ILiteQueryable<TEntity> Query => _context.Database.GetCollection<TEntity>().Query();

        public LiteDbRepository(LiteDbContext context)
        {
            _context = context;
        }
        
        public TEntity Create(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _context.Database.GetCollection<TEntity>().Insert(entity);
            });
            
            return entity;
        }

        public int CreateMany(IEnumerable<TEntity> entities)
        {
            _context.AddCommand(() =>
            {
                _context.Database.GetCollection<TEntity>().InsertBulk(entities);
            });
            
            return entities.Count();
        }

        public TEntity Update(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _context.Database.GetCollection<TEntity>().Update(entity);
            });

            return entity;
        }

        public TEntity Upsert(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _context.Database.GetCollection<TEntity>().Upsert(entity);
            });

            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            _context.AddCommand(() =>
            {
                _context.Database.GetCollection<TEntity>().Delete(entity.Id);
            });

            return entity;
        }
    }
}