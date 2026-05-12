using System;

using SolarPhobia.Application.Resources;

using SolarPhobia.Application.Strike;

using SolarPhobia.Application.Consequences.WaterTrap;

using SolarPhobia.Application.Rituals;

using SolarPhobia.Application.Shrines;

using SolarPhobia.Application.Day;

using SolarPhobia.Application.Flow;

using SolarPhobia.Application.Player.State;

using SolarPhobia.Application.Player.Input;

using SolarPhobia.Application.Player.Interactions;

using SolarPhobia.Application.Player.Cursor;

using SolarPhobia.Application.Player.Events;

using SolarPhobia.Application.Combat;

using SolarPhobia.Application.Phase.Reset;

namespace SolarPhobia.Application.Services.Interfaces
{
    public interface IIllusionPlatformEffectService : IDisposable
    {
        bool IsPlatformCollapsed(string platformId);
        bool IsCollapseTimerActive(string platformId);
        void Tick(float deltaTime);
    }
}

