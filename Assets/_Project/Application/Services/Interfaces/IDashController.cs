// Assets/_Project/Application/Services/Interfaces/IDashController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages Spirit Dash — the Khăn Tang burst skill.
    /// Implements Master GDD V5.0 Section 3.1: Spirit Dash costs -5.0s Ward on activation.
    ///
    /// Rules:
    ///   - Shift key triggers dash (NightMovement mode only)
    ///   - Each dash fires OnWardCostIncurred(5.0f) — Ward Timer deducts the cost
    ///   - Dash blocked when current Ward is insufficient (Ward &lt;= dashCost)
    ///   - Cooldown prevents spam (default 0.5s between dashes)
    ///   - Dash direction follows current horizontal input (or last facing if no input)
    /// </summary>
    public interface IDashController
    {
        /// <summary>Whether a dash is currently in progress.</summary>
        bool IsDashing { get; }

        /// <summary>Whether the dash cooldown has expired and a new dash can be triggered.</summary>
        bool CanDash { get; }

        /// <summary>
        /// Emits the Ward cost when a dash activates.
        /// Payload: cost in seconds (always 5.0f for Spirit Dash).
        /// Ward Timer subscribes to this and deducts the cost.
        /// </summary>
        Observable<float> OnWardCostIncurred { get; }

        /// <summary>Ward cost per dash in seconds. Default: 5.0s.</summary>
        float DashWardCost { get; set; }

        /// <summary>Cooldown between dashes in seconds. Default: 0.5s.</summary>
        float DashCooldown { get; set; }

        /// <summary>
        /// Attempts to trigger a Spirit Dash.
        /// Silently ignored when: mode != NightMovement, cooldown active, or Ward insufficient.
        /// </summary>
        /// <param name="dashInput">Whether the dash key (Shift) is pressed this frame.</param>
        /// <param name="mode">Current player input mode.</param>
        /// <param name="currentWard">Current Ward Timer value — used to check affordability.</param>
        void TryDash(bool dashInput, PlayerInputMode mode, float currentWard);

        /// <summary>
        /// Advances cooldown timer. Call once per frame.
        /// </summary>
        /// <param name="deltaTime">Frame delta time in seconds.</param>
        void Tick(float deltaTime);
    }
}
