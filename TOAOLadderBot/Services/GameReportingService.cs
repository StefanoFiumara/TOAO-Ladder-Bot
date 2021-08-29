using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LiteDB;
using TOAOLadderBot.DataAccess;
using TOAOLadderBot.DataAccess.Models;
using TOAOLadderBot.DataAccess.Repository;

namespace TOAOLadderBot.Services
{
    public class GameReportingService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Match> _matchRepository;
        private readonly int[,] _scoringTable;

        public GameReportingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _playerRepository = _unitOfWork.GetRepository<Player>();
            _matchRepository = _unitOfWork.GetRepository<Match>();
            
            // TODO: Better way of storing scoring table? How about in the DB? Configuration?
            _scoringTable = new[,]
            {
                { 6,  3,   3,  1, 1, 1 },
                { 9,  6,   3,  3, 1, 1 },
                { 12, 9,   6,  3, 3, 1 },
                { 15, 12,  9,  6, 3, 3 },
                { 18, 15, 12,  9, 6, 3 },
                { 21, 18, 15, 12, 9, 6 }
            };
        }

        public async Task<Match> ReportGameAsync(List<IUser> winners, List<IUser> losers)
        {
            var allPlayers = winners.Concat(losers).ToList();

            // TODO: Are exceptions the right way to handle these errors? Should we have custom exceptions?
            if (allPlayers.Distinct().Count() != allPlayers.Count)
                throw new Exception("Found duplicate players in match report, this is not allowed!");

            if (winners.Count != losers.Count)
                throw new Exception("Teams are uneven, this is not allowed!");
            

            return await Task.Run(() =>
            {
                var winnerTeam = FindOrCreateLadderPlayers(winners);
                var loserTeam = FindOrCreateLadderPlayers(losers);

                return ReportGame(winnerTeam, loserTeam);
            });
        }

        private Match ReportGame(List<Player> winners, List<Player> losers)
        {
            var allPlayers = winners.Concat(losers).ToList();
            
            var winnerScoreAvg = winners.Select(p => p.Score).Average();
            var loserScoreAvg  = losers.Select(p => p.Score).Average();
            
            var winnerRank = GetRank(winnerScoreAvg);
            var loserRank = GetRank(loserScoreAvg);

            var points = CalculatePoints(winnerRank, loserRank);

            if (winners.Count > 1)
                points = (int)(points * (1 - 0.05 * allPlayers.Count));
            
            if (points < 1) 
                points = 1;
            
            foreach (var winner in winners)
            {
                winner.Score += points;
                winner.Wins++;

                if (winner.Streak >= 0) winner.Streak++;
                else winner.Streak = 1;
            }

            foreach (var loser in losers)
            {
                loser.Score -= points;
                loser.Losses++;

                if (loser.Streak <= 0) loser.Streak--;
                else loser.Streak = -1;
            }

            var match = new Match
            {
                Id = ObjectId.NewObjectId(),
                Winners = winners.Select(p => p.Name).ToList(),
                Losers = losers.Select(p => p.Name).ToList(),
                ReportedDate = DateTimeOffset.UtcNow
            };

            _matchRepository.Create(match);
            
            foreach (var player in winners.Concat(losers))
            {
                player.MatchHistory.Add(match);
                _playerRepository.Upsert(player);
            }

            // TODO: Test that this logic creates all data correctly.
            _unitOfWork.Save();
            return match;
            
        }

        private Rank GetRank(double score)
        {
            return score switch
            {
                >= 20 and <= 30 => Rank.Newbie,
                >= 31 and <= 60 => Rank.Rook,
                >= 61 and <= 80 => Rank.Grook,
                >= 81 and <= 105 => Rank.Inter,
                >= 106 and <= 154 => Rank.Upper,
                >= 155 => Rank.Expert,
                _ => throw new Exception("Score out of range!") // TODO: Custom exceptions?
            };
        }
        
        private int CalculatePoints(Rank winner, Rank loser)
        {
            return _scoringTable[(int) winner, (int) loser];
        }

        private List<Player> FindOrCreateLadderPlayers(List<IUser> users)
        {
            var players = new List<Player>();

            // NOTE: Map to list of Ids since otherwise the LiteDB provider will try to serialize the entire IUser object
            var playerIds = users.Select(u => u.Id).ToList();
            var existingPlayers = _playerRepository.Query.Where(p => playerIds.Any(id => id == p.DiscordId)).ToList();
            
            var newPlayers = users.Where(u => existingPlayers.All(p => p.DiscordId != u.Id)).ToList();
            
            foreach (var user in newPlayers)
            {
                var player = new Player
                {
                    Id = ObjectId.NewObjectId(),
                    DiscordId = user.Id,
                    Name = user.Username,
                    Score = 75,
                    Wins = 0,
                    Losses = 0,
                    Streak = 0,
                    MatchHistory = new List<Match>()
                };

                players.Add(player);
            }
            
            players.AddRange(existingPlayers);
            
            return players;
        }
    }
}