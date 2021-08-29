using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Discord;
using TOAOLadderBot.DataAccess;
using TOAOLadderBot.DataAccess.Models;

namespace TOAOLadderBot.Tests
{
    public static class TestData
    {
        public static List<IUser> GenerateRandomDiscordUsers(int numUsers)
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true });
            
            List<IUser> result;
            
            do
            {
                result = fixture.CreateMany<IUser>(numUsers).ToList();
            } 
            while (result.Select(u => u.Id).Distinct().Count() != numUsers);

            return result;
        }

        public static (List<IUser> discordUsers, List<Player> ladderPlayers) GenerateRegisteredPlayers(int numPlayers)
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true });
            
            var users = GenerateRandomDiscordUsers(numPlayers);

            var userIds = new Queue<ulong>(users.Select(u => u.Id));

            var players = fixture.Build<Player>()
                .With(p => p.DiscordId, () => userIds.Dequeue())
                .With(p => p.Score, 75)
                .With(p => p.MatchHistory, new List<Match>())
                .CreateMany(users.Count)
                .ToList();

            return (users, players);
        } 
    }
}