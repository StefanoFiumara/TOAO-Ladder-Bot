using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Discord;
using LiteDB;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.Tests
{
    public static class TestData
    {
        public static List<IUser> GenerateUniqueDiscordUsers(int numUsers)
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true });
            
            List<IUser> result;
            
            // NOTE: Not sure if CreateMany will ever generate the same Id for multiple IUsers.
            //       Add a check just in case since we can't control how the read-only property of Discord's IUser interface is generated.
            do
            {
                result = fixture.CreateMany<IUser>(numUsers).ToList();
            } 
            while (result.Select(u => u.Id).Distinct().Count() != numUsers);

            return result;
        }

        public static (List<IUser> discordUsers, List<Player> ladderPlayers) GenerateUniqueRegisteredPlayers(int numPlayers)
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true });
            
            var users = GenerateUniqueDiscordUsers(numPlayers);

            var userIds = new Queue<ulong>(users.Select(u => u.Id));

            var players = fixture.Build<Player>()
                .With(p => p.Id, ObjectId.NewObjectId)
                .With(p => p.DiscordId, () => userIds.Dequeue())
                .With(p => p.Score, 75)
                .With(p => p.MatchHistory, new List<Match>())
                .CreateMany(users.Count)
                .ToList();

            return (users, players);
        } 
    }
}