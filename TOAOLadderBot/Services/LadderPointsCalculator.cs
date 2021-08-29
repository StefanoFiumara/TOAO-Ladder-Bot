using System;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.Services
{
    public static class LadderPointsCalculator
    {
        // TODO: Better way of storing scoring table? How about in the DB? Configuration?
        private static readonly int[,] ScoringTable = {
            { 6,  3,   3,  1, 1, 1 },
            { 9,  6,   3,  3, 1, 1 },
            { 12, 9,   6,  3, 3, 1 },
            { 15, 12,  9,  6, 3, 3 },
            { 18, 15, 12,  9, 6, 3 },
            { 21, 18, 15, 12, 9, 6 }
        };
        
        public static Rank CalculateRank(double score)
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
        
        public static int CalculatePoints(Rank winner, Rank loser, int playerCount)
        {
            var points = ScoringTable[(int) winner, (int) loser];
            
            if (playerCount > 2)
                points = (int)(points * (1 - 0.05 * playerCount));
            
            if (points < 1) 
                points = 1;

            return points;
        }
    }
}