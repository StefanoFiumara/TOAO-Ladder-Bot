using System;
using LiteDB;
using TOAOLadderBot.DataAccess;
using TOAOLadderBot.DataAccess.Models;
using Xunit;

namespace TOAOLadderBot.Tests
{
    public class Tests : IDisposable
    {
        private readonly UnitOfWork _unitOfWork;
        public Tests()
        {
            var db = new LiteDatabase(":memory:");
            var context = new LiteDbContext(db);
            _unitOfWork = new UnitOfWork(db, context);
        }
        
        [Fact]
        public void Canary()
        {
            Assert.True(true);
        }

        [Fact]
        public void CreatePlayer_AssignsObjectId()
        {
            var repo = _unitOfWork.GetRepository<Player>();

            var player = new Player
            {
                Id = ObjectId.Empty,
                Name = "Fano",
                Score = 75
            };

            repo.Create(player);

            _unitOfWork.Save();

            var allPlayers = repo.Query.ToList();
            var createdPlayer = Assert.Single(allPlayers);
            Assert.True(createdPlayer.Id != ObjectId.Empty);
        }
        
        [Fact]
        public void CreatePlayer_WithDuplicateObjectId_Fails()
        {
            var repo = _unitOfWork.GetRepository<Player>();

            var player = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "Fano",
                Score = 75
            };

            repo.Create(player);
            repo.Create(player);

            Assert.Throws<LiteException>(() => _unitOfWork.Save());
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}