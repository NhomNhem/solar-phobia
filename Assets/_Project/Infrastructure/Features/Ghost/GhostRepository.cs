using System.Collections.Generic;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Repositories;

namespace SolarPhobia.Infrastructure.Features.Ghost
{
    /// <summary>
    /// In-memory repository for ghost entities used by combat and consequence flows.
    /// </summary>
    public class GhostRepository : IGhostRepository
    {
        private readonly Dictionary<string, SolarPhobia.Domain.Ghost> _ghosts;

        public GhostRepository()
        {
            _ghosts = new Dictionary<string, SolarPhobia.Domain.Ghost>
            {
                ["linh"] = new SolarPhobia.Domain.Ghost("linh", "Em Linh"),
                ["van"] = new SolarPhobia.Domain.Ghost("van", "Ông Văn"),
                ["minh"] = new SolarPhobia.Domain.Ghost("minh", "Anh Minh")
            };
        }

        public IEnumerable<SolarPhobia.Domain.Ghost> GetAll()
        {
            return _ghosts.Values;
        }

        public SolarPhobia.Domain.Ghost GetById(string id)
        {
            return _ghosts.TryGetValue(id, out var ghost) ? ghost : null;
        }

        public void Save(SolarPhobia.Domain.Ghost ghost)
        {
            if (ghost == null)
            {
                return;
            }

            _ghosts[ghost.Id] = ghost;
        }
    }
}
