using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LiteDB;
using TOAOLadderBot.DataAccess;
using TOAOLadderBot.DataAccess.Repository;
using TOAOLadderBot.Exceptions;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.Services
{
    public class GameReportingService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Match> _matchRepository;

        public GameReportingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _playerRepository = _unitOfWork.GetRepository<Player>();
            _matchRepository = _unitOfWork.GetRepository<Match>();   
        }

        public async Task<Match> ReportGameAsync(List<IUser> winners, List<IUser> losers)
        {
            var allPlayers = winners.Concat(losers).ToList();
            
            if (allPlayers.Distinct().Count() != allPlayers.Count)
                throw new GameReportException("Found duplicate players in match report, this is not allowed!");

            if (winners.Count != losers.Count)
                throw new GameReportException("Teams are uneven, this is not allowed!");
            

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
            
            var winnerRank = LadderPointsCalculator.CalculateRank(winnerScoreAvg);
            var loserRank = LadderPointsCalculator.CalculateRank(loserScoreAvg);

            var points = LadderPointsCalculator.CalculatePoints(winnerRank, loserRank, allPlayers.Count);

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
                Winners = winners.ToList(),
                Losers = losers.ToList(),
                ReportedDate = DateTimeOffset.UtcNow
            };

            _matchRepository.Create(match);
            
            foreach (var player in winners.Concat(losers))
            {
                player.MatchHistory.Add(match);
                _playerRepository.Upsert(player);
            }
            
            _unitOfWork.Save();
            return match;
            
        }

        private List<Player> FindOrCreateLadderPlayers(List<IUser> users)
        {
            var players = new List<Player>();

            // NOTE: Map to list of Ids since otherwise the LiteDB provider will try to serialize the entire IUser object when we query
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