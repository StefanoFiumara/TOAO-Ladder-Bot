using LiteDB;

namespace TOAOLadderBot.DataAccess
{
    public interface ILiteDbDocument
    {
        [BsonId]
        ObjectId Id { get; init; }
    }
}