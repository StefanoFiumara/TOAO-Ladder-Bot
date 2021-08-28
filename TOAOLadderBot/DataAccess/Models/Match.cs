using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace TOAOLadderBot.DataAccess.Models
{
    public class Match : ILiteDbDocument, IEquatable<Match>
    {
        public ObjectId Id { get; init; }
        
        public DateTimeOffset ReportedDate { get; set; }
        
        // TODO: Do we need more info than just winners/losers? LiteDB does not allow circular references so this might get messy.
        public List<string> Winners { get; set; }
        public List<string> Losers { get; set; }
        
        public override string ToString()
        {
            var winners = string.Join(", ", Winners);
            var losers = string.Join(", ", Losers);

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