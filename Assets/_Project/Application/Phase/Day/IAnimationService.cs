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

namespace SolarPhobia.Application.Phase.Day
{
    /// <summary>
    /// Service responsible for playing animation effects.
    /// </summary>
    public interface IAnimationService
    {
        void PlaySwapAnimation(string playerId, string soulId, float duration);
        void PlayShoveAnimation(string playerId, string soulId);
    }
}
