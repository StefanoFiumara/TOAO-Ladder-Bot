using System;
using System.Collections.Generic;
using LiteDB;
using TOAOLadderBot.Services;

namespace TOAOLadderBot.Models
{
    public class Player : ILiteDbDocument, IEquatable<Player>
    {
        public ObjectId Id { get; init; }
        public ulong DiscordId { get; init; }
        
        public string Name { get; set; }
        
        public int Score { get; set; }
        
        public int Streak { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        
        public List<Match> MatchHistory { get; set; }
        
        public int GamesPlayed => Wins + Losses;

        public Rank Rank => LadderPointsCalculator.CalculateRank(Score);
        public float WinPercent => GamesPlayed > 0 ? (float) Wins / GamesPlayed : 0.0f;

        public bool Equals(Player other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Player)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
 }