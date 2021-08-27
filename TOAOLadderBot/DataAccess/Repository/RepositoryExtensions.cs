using LiteDB;
using TOAOLadderBot.DataAccess.Models;

namespace TOAOLadderBot.DataAccess.Repository
{
    public static class RepositoryExtensions
    {
        public static Player FindByName(this IRepository<Player> repo, string name)
        {
            // TODO: Null check?
            return repo.Query.Where(p => p.Name.Contains(name)).SingleOrDefault();
        }
    }
}