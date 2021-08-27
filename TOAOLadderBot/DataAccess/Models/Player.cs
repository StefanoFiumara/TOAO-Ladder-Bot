using System.Collections.Generic;
using LiteDB;

namespace TOAOLadderBot.DataAccess.Models
{
    public class Player : IDbModel
    {
        public ObjectId Id { get; set; }
        
        public string Name { get; set; } // TODO: Ensure Index by name
        
        public int Score { get; set; }
        
        public int Streak { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        
        [BsonRef(nameof(Match))]
        private List<Match> MatchHistory { get; set; }
        
        public int GamesPlayed => Wins + Losses;
        
        // TODO: Rank/Tier calculated from score? or based on percentile?
        // TODO: Win Percent
    }
 }