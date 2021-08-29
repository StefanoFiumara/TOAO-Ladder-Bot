using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using TOAOLadderBot.DataAccess.Models;
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
            var users = TestData.GenerateRandomDiscordUsers(2);

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
            var (users, players) = TestData.GenerateRegisteredPlayers(2);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act
            await _service.ReportGameAsync(users.Take(1).ToList(), users.TakeLast(1).ToList());

            // Assert
            var result = repository.Query.OrderBy(p => p.Name).ToList();
            Assert.Equal(2, result.Count);
        }
    }
}