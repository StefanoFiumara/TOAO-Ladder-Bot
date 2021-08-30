using System;
using TOAOLadderBot.DataAccess.Repository;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.DataAccess
{
    public sealed class UnitOfWork : IDisposable
    {
        private readonly LiteDbContext _context;

        public UnitOfWork(LiteDbContext context)
        {
            _context = context;

            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            _context.Database.GetCollection<Player>().EnsureIndex(p => p.DiscordId);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, ILiteDbDocument
            => new LiteDbRepository<TEntity>(_context);

        public int Save() => _context.Save();

        public int Rollback() => _context.Rollback();

        public void Dispose() => _context.Database?.Dispose();
    }
}