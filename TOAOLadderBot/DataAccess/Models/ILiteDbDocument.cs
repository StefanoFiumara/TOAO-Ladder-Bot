using LiteDB;

namespace TOAOLadderBot.DataAccess.Models
{
    public interface ILiteDbDocument
    {
        [BsonId]
        ObjectId Id { get; set; }
    }
}