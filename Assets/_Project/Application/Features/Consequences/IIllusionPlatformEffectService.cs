using System;

namespace SolarPhobia.Application.Features.Consequences
{
    public interface IIllusionPlatformEffectService : IDisposable
    {
        bool IsPlatformCollapsed(string platformId);
        bool IsCollapseTimerActive(string platformId);
        void Tick(float deltaTime);
    }
}
