using System;
using LiteDB;
using TOAOLadderBot.DataAccess;

namespace TOAOLadderBot.Tests
{
    public abstract class LadderBotTestsBase : IDisposable
    {
        protected readonly UnitOfWork UnitOfWork;

        protected LadderBotTestsBase()
        {
            var db = new LiteDatabase(":memory:");
            var context = new LiteDbContext(db);
            UnitOfWork = new UnitOfWork(db, context);
        }
        public void Dispose()
        {
            UnitOfWork?.Dispose();
        }
    }
}