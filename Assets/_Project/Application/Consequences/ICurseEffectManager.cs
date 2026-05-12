using System;
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Consequences
{
    /// <summary>
    /// Interface for curse effect management - controls hazard spawning based on abandoned soul.
    /// </summary>
    public interface ICurseEffectManager : IDisposable
    {
        /// <summary>
        /// Current curse effect state value (Idle/CurseActive/HazardTriggered/HazardCleared).
        /// </summary>
        CurseEffectState CurrentStateValue { get; }

        /// <summary>
        /// Current curse effect state observable.
        /// </summary>
        ReadOnlyReactiveProperty<CurseEffectState> CurrentState { get; }

        /// <summary>
        /// Observable that triggers when a hazard is triggered (player enters hazard zone).
        /// </summary>
        Observable<HazardEvent> OnHazardTriggered { get; }

        /// <summary>
        /// Observable that triggers when a hazard is cleared (player exits hazard zone).
        /// </summary>
        Observable<HazardEvent> OnHazardCleared { get; }

        /// <summary>
        /// Initialize the curse effect manager and subscribe to phase changes.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Receive curse type from Consequence Resolver at night start.
        /// </summary>
        void ReceiveCurse(NightOutcomeState curseType);

        /// <summary>
        /// Handle player entering a hazard zone.
        /// </summary>
        void OnPlayerEnterHazard(string hazardId);

        /// <summary>
        /// Handle player exiting a hazard zone.
        /// </summary>
        void OnPlayerExitHazard(string hazardId);
    }

    /// <summary>
    /// Curse effect states for the state machine.
    /// </summary>
    public enum CurseEffectState
    {
        Idle,
        CurseActive,
        HazardTriggered,
        HazardCleared
    }

    /// <summary>
    /// Event data for hazard trigger/clear events.
    /// </summary>
    public struct HazardEvent
    {
        public string HazardId { get; set; }
        public NightOutcomeState CurseType { get; set; }
    }
}
