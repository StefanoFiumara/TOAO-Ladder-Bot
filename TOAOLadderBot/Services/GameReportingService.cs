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

        public GameReportingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _playerRepository = _unitOfWork.GetRepository<Player>();
            _matchRepository = _unitOfWork.GetRepository<Match>();
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
            // TODO: point calculation by rank
            var scoringTable = new Dictionary<Rank, Dictionary<Rank, int>>
            {
                { Rank.Expert, new Dictionary<Rank, int>() },
                { Rank.Upper, new Dictionary<Rank, int>() },
                { Rank.Inter, new Dictionary<Rank, int>() },
                { Rank.Grook, new Dictionary<Rank, int>() },
                { Rank.Rook, new Dictionary<Rank, int>() },
                { Rank.Newbie, new Dictionary<Rank, int>() }
            };
            
            // TODO: this will fail unless we add all the keys to the inner dictionaries
            // TODO: Better way of storing scoring table? How about in the DB? Configuration?
            scoringTable[Rank.Expert].Add(Rank.Expert, 6); // etc...

            
            scoringTable[Rank.Expert][Rank.Expert] = 6;
            scoringTable[Rank.Expert][Rank.Upper] = 3;
            scoringTable[Rank.Expert][Rank.Inter] = 3;
            scoringTable[Rank.Expert][Rank.Grook] = 1;
            scoringTable[Rank.Expert][Rank.Rook] = 1;
            scoringTable[Rank.Expert][Rank.Newbie] = 1;

            scoringTable[Rank.Upper][Rank.Expert] = 9;
            scoringTable[Rank.Upper][Rank.Upper] = 6;
            scoringTable[Rank.Upper][Rank.Inter] = 3;
            scoringTable[Rank.Upper][Rank.Grook] = 3;
            scoringTable[Rank.Upper][Rank.Rook] = 1;
            scoringTable[Rank.Upper][Rank.Newbie] = 1;
            
            scoringTable[Rank.Inter][Rank.Expert] = 12;
            scoringTable[Rank.Inter][Rank.Upper] = 9;
            scoringTable[Rank.Inter][Rank.Inter] = 6;
            scoringTable[Rank.Inter][Rank.Grook] = 3;
            scoringTable[Rank.Inter][Rank.Rook] = 3;
            scoringTable[Rank.Inter][Rank.Newbie] = 1;
            
            scoringTable[Rank.Grook][Rank.Expert] = 15;
            scoringTable[Rank.Grook][Rank.Upper] = 12;
            scoringTable[Rank.Grook][Rank.Inter] = 9;
            scoringTable[Rank.Grook][Rank.Grook] = 6;
            scoringTable[Rank.Grook][Rank.Rook] = 3;
            scoringTable[Rank.Grook][Rank.Newbie] = 3;
            
            scoringTable[Rank.Rook][Rank.Expert] = 18;
            scoringTable[Rank.Rook][Rank.Upper] = 15;
            scoringTable[Rank.Rook][Rank.Inter] = 12;
            scoringTable[Rank.Rook][Rank.Grook] = 9;
            scoringTable[Rank.Rook][Rank.Rook] = 6;
            scoringTable[Rank.Rook][Rank.Newbie] = 3;
            
            scoringTable[Rank.Newbie][Rank.Expert] = 21;
            scoringTable[Rank.Newbie][Rank.Upper] = 18;
            scoringTable[Rank.Newbie][Rank.Inter] = 15;
            scoringTable[Rank.Newbie][Rank.Grook] = 12;
            scoringTable[Rank.Newbie][Rank.Rook] = 9;
            scoringTable[Rank.Newbie][Rank.Newbie] = 6;
            
            return scoringTable[winner][loser];
        }

        private List<Player> FindOrCreateLadderPlayers(List<IUser> users)
        {
            var players = new List<Player>();

            // TODO: Test that this method doesn't create any new players when all players are already signed up for the ladder
            var existingPlayers = _playerRepository.Query.Where(p => users.Any(u => u.Id == p.DiscordId)).ToList();
            var missingUsers = users.Where(u => existingPlayers.All(p => p.DiscordId != u.Id)).ToList();
            
            foreach (var user in missingUsers)
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
            
            return players;
        }
    }
}