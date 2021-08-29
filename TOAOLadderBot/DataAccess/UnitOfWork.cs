using System;
using LiteDB;
using TOAOLadderBot.DataAccess.Repository;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.DataAccess
{
    public sealed class UnitOfWork : IDisposable
    {
        private readonly LiteDbContext _context;
        private readonly LiteDatabase _database;

        public UnitOfWork(LiteDatabase database, LiteDbContext context)
        {
            _context = context;
            _database = database;

            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            _database.GetCollection<Player>().EnsureIndex(p => p.DiscordId);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, ILiteDbDocument
            => new LiteDbRepository<TEntity>(_context, _database);

        public int Save() => _context.Save();

        public int Rollback() => _context.Rollback();

        public void Dispose() => _database?.Dispose();
    }
}