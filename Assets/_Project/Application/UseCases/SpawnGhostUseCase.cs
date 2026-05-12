using System.Linq;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Repositories;

namespace SolarPhobia.Application.UseCases
{
    public class SpawnGhostUseCase
    {
        private readonly IGhostRepository _ghostRepository;

        public SpawnGhostUseCase(IGhostRepository ghostRepository)
        {
            _ghostRepository = ghostRepository;
        }

        public Ghost Execute()
        {
            return _ghostRepository.GetAll()
                .FirstOrDefault(ghost => ghost != null && ghost.VisitCount < 3);
        }
    }
}
