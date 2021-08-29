using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
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
        public async Task GameReport_CreatesMatchData_CorrectlyAssignsWinnersAndLosers()
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
    }
}