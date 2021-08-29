using System.Linq;
using System.Threading.Tasks;
using TOAOLadderBot.Exceptions;
using TOAOLadderBot.Models;
using TOAOLadderBot.Services;
using Xunit;

namespace TOAOLadderBot.Tests
{
    public class GameReportingTests : LadderBotTestsBase
    {
        private readonly GameReportingService _service;

        public GameReportingTests()
        {
            _service = new GameReportingService(UnitOfWork);
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
            var winnerUser = users.Take(1).ToList();
            var loserUser = users.TakeLast(1).ToList();
            await _service.ReportGameAsync(winnerUser, loserUser);

            // Assert
            var result = repository.Query.OrderBy(p => p.Name).ToList();
            Assert.Equal(2, result.Count);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(8)]
        public async Task GameReport_CreatesMatchData_CorrectlyAssignsWinnersAndLosers(int numPlayers)
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
        public async Task GameReport_CorrectlySetsStreak()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(2);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act
            var winnerUser = users.Take(1).ToList();
            var loserUser = users.TakeLast(1).ToList();
            
            await _service.ReportGameAsync(winnerUser, loserUser);
            await _service.ReportGameAsync(winnerUser, loserUser);
            await _service.ReportGameAsync(winnerUser, loserUser);
            
            // Assert
            var winnerId = winnerUser.Single().Id;
            var loserId = loserUser.Single().Id;

            var winnerPlayer = repository.Query.Where(p => p.DiscordId == winnerId).Single();
            var loserPlayer  = repository.Query.Where(p => p.DiscordId == loserId).Single();
            
            Assert.Equal(3, winnerPlayer.Streak);
            Assert.Equal(-3, loserPlayer.Streak);
        }
        
        [Fact]
        public async Task GameReport_Loser_CorrectlyLosesStreak()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(2);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act
            var player1 = users.Take(1).ToList();
            var player2 = users.TakeLast(1).ToList();
            
            await _service.ReportGameAsync(player1, player2);
            await _service.ReportGameAsync(player1, player2);
            await _service.ReportGameAsync(player2, player1);
            
            // Assert
            var lostStreakId = player1.Single().Id;
            var lostStreakPlayer = repository.Query.Where(p => p.DiscordId == lostStreakId).Single();

            Assert.Equal(-1, lostStreakPlayer.Streak);
        }
        
        [Fact]
        public async Task GameReport_Winner_CorrectlyGainsStreak()
        {
            // Arrange
            var (users, players) = TestData.GenerateUniqueRegisteredPlayers(2);
            
            var repository = UnitOfWork.GetRepository<Player>();

            repository.CreateMany(players);
            UnitOfWork.Save();

            // Act
            var player1 = users.Take(1).ToList();
            var player2 = users.TakeLast(1).ToList();
            
            await _service.ReportGameAsync(player1, player2);
            await _service.ReportGameAsync(player1, player2);
            await _service.ReportGameAsync(player2, player1);
            
            // Assert
            var wonStreakId = player2.Single().Id;
            var wonStreakPlayer = repository.Query.Where(p => p.DiscordId == wonStreakId).Single();

            Assert.Equal(1, wonStreakPlayer.Streak);
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