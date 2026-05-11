// Assets/_Project/Application/Services/Interfaces/ISwingGlideController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages Swing and Glide skills — Khăn Tang cloth movement abilities.
    /// Implements Master GDD V5.0 Section 3.1.
    ///
    /// Swing:  Left Click near anchor → grapple arc, costs -2.0s Ward on attach.
    /// Glide:  Hold (airborne) → reduced gravity + horizontal drift, costs -1.0s/sec Ward.
    ///
    /// Both skills blocked when mode != NightMovement.
    /// Both fire OnWardCostIncurred — Ward Timer subscribes and deducts.
    /// </summary>
    public interface ISwingGlideController
    {
        /// <summary>Whether Swing is currently active (attached to anchor).</summary>
        bool IsSwinging { get; }

        /// <summary>Whether Glide is currently active.</summary>
        bool IsGliding { get; }

        /// <summary>
        /// Emits Ward cost when a skill activates or ticks.
        /// Swing: fires once on attach (2.0f).
        /// Glide: fires each frame while active (1.0f * deltaTime).
        /// </summary>
        Observable<float> OnWardCostIncurred { get; }

        /// <summary>Ward cost for Swing activation. Default: 2.0s.</summary>
        float SwingWardCost { get; set; }

        /// <summary>Ward cost per second while Gliding. Default: 1.0s/sec.</summary>
        float GlideWardCostPerSec { get; set; }

        /// <summary>
        /// Attempts to start a Swing.
        /// Blocked when: mode != NightMovement, Ward insufficient, or already swinging.
        /// </summary>
        void TrySwing(bool swingInput, PlayerInputMode mode, float currentWard);

        /// <summary>Releases the Swing anchor.</summary>
        void ReleaseSwing();

        /// <summary>
        /// Updates Glide state each frame.
        /// Glide activates when: glideInput held AND airborne AND mode == NightMovement.
        /// Fires Ward cost each frame while active.
        /// Auto-cancels when Ward reaches 0.
        /// </summary>
        void TickGlide(bool glideInput, bool isAirborne, PlayerInputMode mode, float currentWard, float deltaTime);
    }
}
