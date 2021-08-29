using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using TOAOLadderBot.Exceptions;
using TOAOLadderBot.Models;
using TOAOLadderBot.Services;
using Xunit;

namespace TOAOLadderBot.Tests
{
    public class GameReportingTests : LadderBotTestsBase
    {
        private readonly GameReportingService _service;
        private readonly IFixture _fixture;

        public GameReportingTests()
        {
            _service = new GameReportingService(UnitOfWork);
            _fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true });
        }

        [Fact]
        public async Task GameReport_WithNewPlayers_CreatesNewPlayers()
        {
            // Arrange
            var users = TestData.GenerateUniqueDiscordUsers(2);

            // Act
            await _service.ReportGameAsync(users.Take(1).ToList(), users.TakeLast(1).ToList());

            // Assert
            var result = UnitOfWork.GetRepository<Player>().Query.OrderBy(p => p.Name).ToList();
            Assert.Equal(2, result.Count);
        }
        
        [Fact]
        public async Task GameReport_WithExistingPlayers_DoesNotCreateNewPlayers()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(2);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act
            await _service.ReportGameAsync(users.Take(1).ToList(), users.TakeLast(1).ToList());

            // Assert
            var result = repository.Query.OrderBy(p => p.Name).ToList();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GameReport_CreatesMatchData_1vs1_CorrectlyAssignsWinnersAndLosers()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(2);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();
            
            // Act
            var match = await _service.ReportGameAsync(users.Take(1).ToList(), users.TakeLast(1).ToList());
            
            // Assert
            Assert.Equal(players.First().Id, match.Winners.Single().Id);
            Assert.Equal(players.Last().Id, match.Losers.Single().Id);
        }
        
        [Theory]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(8)]
        public async Task GameReport_CreatesMatchData_TeamGame_CorrectlyAssignsWinnersAndLosers(int numPlayers)
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(numPlayers);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();
            
            // Act
            var winnerUsers = users.Take(numPlayers / 2).ToList();
            var loserUsers = users.TakeLast(numPlayers / 2).ToList();
            var match = await _service.ReportGameAsync(winnerUsers, loserUsers);
            
            // Assert
            Assert.All(match.Winners, winner =>
            {
                Assert.Contains(winnerUsers, u => u.Id == winner.DiscordId);
            });
            
            Assert.All(match.Losers, loser =>
            {
                Assert.Contains(loserUsers, u => u.Id == loser.DiscordId);
            });
        }

        [Fact]
        public async Task GameReport_WithEmptyTeam_Throws()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(3);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act / Assert
            await Assert.ThrowsAsync<GameReportException>(() => _service.ReportGameAsync(users.Take(0).ToList(), users.TakeLast(0).ToList()));
        }
        
        [Fact]
        public async Task GameReport_WithUnevenTeams_Throws()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(3);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act / Assert
            await Assert.ThrowsAsync<GameReportException>(() => _service.ReportGameAsync(users.Take(2).ToList(), users.TakeLast(1).ToList()));
            await Assert.ThrowsAsync<GameReportException>(() => _service.ReportGameAsync(users.Take(1).ToList(), users.TakeLast(2).ToList()));
        }
        
        [Fact]
        public async Task GameReport_WithDuplicatePlayer_Throws()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(2);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act / Assert
            await Assert.ThrowsAsync<GameReportException>(() => _service.ReportGameAsync(users.Take(1).ToList(), users.Take(1).ToList()));
        }
    }
}