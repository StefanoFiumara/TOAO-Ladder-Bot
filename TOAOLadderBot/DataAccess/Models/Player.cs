using System;
using System.Collections.Generic;
using LiteDB;

namespace TOAOLadderBot.DataAccess.Models
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
        
        [BsonRef(nameof(Match))]
        private List<Match> MatchHistory { get; set; }
        
        public int GamesPlayed => Wins + Losses;
        
        // TODO: Rank/Tier calculated from score? or based on percentile?
        // TODO: Win Percent
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