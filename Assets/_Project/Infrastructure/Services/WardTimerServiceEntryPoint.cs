using VContainer.Unity;

namespace SolarPhobia.Infrastructure.Services
{
    /// <summary>
    /// Bridges the ward timer singleton into VContainer entry point lifecycle callbacks.
    /// </summary>
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
