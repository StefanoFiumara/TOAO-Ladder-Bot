using LiteDB;

namespace TOAOLadderBot.Models
{
    public interface ILiteDbDocument
    {
        [BsonId]
        ObjectId Id { get; init; }
    }
}