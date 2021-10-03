using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDB;
using TOAOLadderBot.DataAccess;

namespace TOAOLadderBot.Models
{
    public class Match : ILiteDbDocument, IEquatable<Match>
    {
        public ObjectId Id { get; init; }
        
        public DateTimeOffset ReportedDate { get; init; }
        
        public int PointsAwarded { get; init; }
        
        [BsonRef(nameof(Player))]
        public List<Player> Winners { get; init; }
        
        [BsonRef(nameof(Player))]
        public List<Player> Losers { get; init; }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            var winners = string.Join("\n", Winners.Select(w => w.Name));
            var losers = string.Join("\n", Losers.Select(l => l.Name));

            var s = Winners.Count == 1 ? "s" : string.Empty;
            var notS = Winners.Count != 1 ? "s" : string.Empty;

            sb.AppendLine($"\n{winners}\n---- DEFEAT{s.ToUpper()} ----\n{losers}\n");
            sb.AppendLine($"This match was recorded on {ReportedDate:yyyy-MM-dd hh:mm tt \"UTC\"}");
            sb.AppendLine($"The winner{notS} take{s} {PointsAwarded} points from the loser{notS}, GG!");
            
            return sb.ToString();
        }

        public string ToHistoryString(ulong povDiscordId)
        {
            if (Winners.Any(p => p.DiscordId == povDiscordId))
            {
                return $"{ReportedDate:d} - Won vs {string.Join(", ", Losers.Select(l => l.Name))} | +{PointsAwarded}";
            }

            if (Losers.Any(p => p.DiscordId == povDiscordId))
            {
                return $"{ReportedDate:d} - Lost vs {string.Join(", ", Winners.Select(l => l.Name))} | -{PointsAwarded}";
            }

            return string.Empty;
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