using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace TOAOLadderBot.DataAccess.Models
{
    public class Match : IDbModel
    {
        public ObjectId Id { get; set; }
        
        private List<Player> Winners { get; set; }
        private List<Player> Losers { get; set; }
        
        private DateTimeOffset ReportedDate { get; set; }
        
        public override string ToString()
        {
            var winners = string.Join(" ", Winners.Select(p => p.Name));
            var losers = string.Join(" ", Losers.Select(p => p.Name));

            var s = Winners.Count == 1 ? "s" : string.Empty;
            return $"{ReportedDate:g} -- {winners} defeat{s} {losers}";
        }
    }
}