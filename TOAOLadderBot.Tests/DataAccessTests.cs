using System;
using LiteDB;
using TOAOLadderBot.DataAccess.Models;
using TOAOLadderBot.DataAccess.Repository;
using Xunit;

namespace TOAOLadderBot.Tests
{
    public class DataAccessTests : LadderBotTestsBase, IDisposable
    {
        [Fact]
        public void Create_AssignsObjectId()
        {
            // Arrange
            var repo = UnitOfWork.GetRepository<Player>();
            var player = new Player
            {
                Id = ObjectId.Empty,
                Name = "Fano",
                Score = 75
            };

            // Act
            repo.Create(player);
            UnitOfWork.Save();

            // Assert
            var allPlayers = repo.Query.ToList();
            var createdPlayer = Assert.Single(allPlayers);
            Assert.True(createdPlayer.Id != ObjectId.Empty);
        }
        
        [Fact]
        public void Create_WithDuplicateObjectId_Fails()
        {
            // Arrange
            var repo = UnitOfWork.GetRepository<Player>();
            var player = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "Fano",
                Score = 75
            };

            // Act
            repo.Create(player);
            repo.Create(player);

            // Assert
            Assert.Throws<LiteException>(() => UnitOfWork.Save());
            Assert.Empty(repo.Query.ToList());
        }

        [Fact]
        public void Update_ReplacesObjects()
        {
            // Arrange
            var repo = UnitOfWork.GetRepository<Player>();
            var player = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "Fano",
                Score = 75
            };

            repo.Create(player);
            UnitOfWork.Save();

            // Act
            player.Score = 80;
            repo.Update(player);

            UnitOfWork.Save();

            // Assert
            var saved = Assert.Single(repo.Query.ToList());
            Assert.Equal("Fano", saved.Name);
            Assert.Equal(80, saved.Score);
        }

        [Fact]
        public void Delete_RemovesObject()
        {
            // Arrange
            var repo = UnitOfWork.GetRepository<Player>();
            var player = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "Fano",
                Score = 75
            };

            repo.Create(player);
            UnitOfWork.Save();

            // Act
            repo.Delete(player);
            UnitOfWork.Save();
            
            // Assert
            Assert.Empty(repo.Query.ToList());
        }

        [Fact]
        public void FindByName_FiltersCorrectly()
        {
            // Arrange
            var repo = UnitOfWork.GetRepository<Player>();
            var fano = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | Fano",
                Score = 75
            };

            var nemo = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | Nemo",
                Score = 75
            };
            
            var bryan = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | 1nsane",
                Score = 75
            };
            
            repo.Create(fano);
            repo.Create(nemo);
            repo.Create(bryan);
            UnitOfWork.Save();
            
            // Act
            var search1 = repo.FindByName("Fano");
            var search2 = repo.FindByName("Nemo");
            var search3 = repo.FindByName("1nsane");
            
            Assert.NotNull(search1);
            Assert.NotNull(search2);
            Assert.NotNull(search3);
            
            Assert.Equal(fano.Id, search1.Id);
            Assert.Equal(nemo.Id, search2.Id);
            Assert.Equal(bryan.Id, search3.Id);
        }

        [Fact]
        public void FindByName_ForNoMatch_ReturnsNull()
        {
            // Arrange
            var repo = UnitOfWork.GetRepository<Player>();
            var fano = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | Fano",
                Score = 85
            };

            var nemo = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | Nemo",
                Score = 75
            };
            
            var bryan = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | 1nsane",
                Score = 65
            };
            
            repo.Create(fano);
            repo.Create(nemo);
            repo.Create(bryan);
            UnitOfWork.Save();
            
            // Act
            var search = repo.FindByName("Cheesus");
            
            // Assert
            Assert.Null(search);
        }

        [Fact]
        public void Query_ReturnsFilteredData()
        {
            // Arrange
            var repo = UnitOfWork.GetRepository<Player>();
            var fano = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | Fano",
                Score = 85
            };

            var nemo = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | Nemo",
                Score = 75
            };
            
            var bryan = new Player
            {
                Id = ObjectId.NewObjectId(),
                Name = "TOAO | 1nsane",
                Score = 65
            };
            
            repo.Create(fano);
            repo.Create(nemo);
            repo.Create(bryan);
            UnitOfWork.Save();
            
            // Act
            var searchResult = repo.Query.Where(p => p.Score > 70).ToList();
            
            // Assert
            Assert.NotNull(searchResult);
            Assert.Equal(2, searchResult.Count);
            
            Assert.Contains(fano, searchResult);
            Assert.Contains(nemo, searchResult);
        }
    }
}