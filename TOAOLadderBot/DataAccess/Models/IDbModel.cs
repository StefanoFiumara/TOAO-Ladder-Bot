using LiteDB;

namespace TOAOLadderBot.DataAccess.Models
{
    public interface IDbModel
    {
        [BsonId]
        ObjectId Id { get; set; }
    }
}