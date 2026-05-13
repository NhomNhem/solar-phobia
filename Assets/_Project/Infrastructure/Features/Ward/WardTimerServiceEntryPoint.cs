using VContainer.Unity;

namespace SolarPhobia.Infrastructure.Features.Ward
{
    public sealed class WardTimerServiceEntryPoint : IInitializable, ITickable
    {
        private readonly WardTimerService _wardTimerService;

        public WardTimerServiceEntryPoint(WardTimerService wardTimerService)
        {
            _wardTimerService = wardTimerService;
        }

        public void Initialize()
        {
            _wardTimerService.Initialize();
        }

        public void Tick()
        {
            _wardTimerService.Tick();
        }
    }
}
