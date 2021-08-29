using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TOAOLadderBot.DataAccess;

namespace TOAOLadderBot.Models
{
    public class Match : ILiteDbDocument, IEquatable<Match>
    {
        public ObjectId Id { get; init; }
        
        public DateTimeOffset ReportedDate { get; init; }
        
        [BsonRef(nameof(Player))]
        public List<Player> Winners { get; init; }
        
        [BsonRef(nameof(Player))]
        public List<Player> Losers { get; init; }
        
        public override string ToString()
        {
            var winners = string.Join(", ", Winners.Select(w => w.Name));
            var losers = string.Join(", ", Losers.Select(l => l.Name));

            var s = Winners.Count == 1 ? "s" : string.Empty;
            return $"{ReportedDate:g} -- {winners} defeat{s} {losers}";
        }

        public bool Equals(Match other)
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
            return Equals((Match)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}