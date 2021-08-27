using System.Collections.Generic;
using System.Linq;

namespace TOAOLadderBot.DataAccess.Models
{
    public class Match
    {
        private List<Player> Winners { get; set; }
        private List<Player> Losers { get; set; }
        
        public override string ToString()
        {
            var winners = string.Join(" ", Winners.Select(p => p.Name));
            var losers = string.Join(" ", Losers.Select(p => p.Name));

            var s = Winners.Count == 1 ? "s" : string.Empty;
            return $"{winners} defeat{s} {losers}";
        }
    }
}