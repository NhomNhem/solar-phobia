using System;
using R3;
using SolarPhobia.Application.Consequences;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Hazards;
using SolarPhobia.Application.Phase.Day;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Phase.Timeline;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Services;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.Services;
using SolarPhobia.Domain.ValueObjects;
using VContainer.Unity;

namespace SolarPhobia.Application.Phase.Reset
{
    /// <summary>
    /// Coordinates the end-of-night reset flow and returns the run to DayService.
    /// </summary>
    public sealed class NightToDayResetService : IInitializable, IDisposable
    {
        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly ISoulRepository _soulRepository;
        private readonly IDayPhaseMechanicsService _dayPhaseMechanicsService;
        private readonly IDayPhaseTimelineService _dayPhaseTimelineService;
        private readonly IDayServiceUIController _dayServiceUIController;
        private readonly INgocCotService _ngocCotService;
        private readonly IKarmaHazardService _karmaHazardService;
        private readonly IConsequenceResolver _consequenceResolver;
        private readonly IWardDeathTriggerService _wardDeathTriggerService;
        private IDisposable _resolveSubscription;
        private bool _isResetting;

        public NightToDayResetService(
            IPhaseStateMachine phaseStateMachine,
            ISoulRepository soulRepository,
            IDayPhaseMechanicsService dayPhaseMechanicsService,
            IDayPhaseTimelineService dayPhaseTimelineService,
            IDayServiceUIController dayServiceUIController,
            INgocCotService ngocCotService,
            IKarmaHazardService karmaHazardService,
            IConsequenceResolver consequenceResolver,
            IWardDeathTriggerService wardDeathTriggerService)
        {
            _phaseStateMachine = phaseStateMachine;
            _soulRepository = soulRepository;
            _dayPhaseMechanicsService = dayPhaseMechanicsService;
            _dayPhaseTimelineService = dayPhaseTimelineService;
            _dayServiceUIController = dayServiceUIController;
            _ngocCotService = ngocCotService;
            _karmaHazardService = karmaHazardService;
            _consequenceResolver = consequenceResolver;
            _wardDeathTriggerService = wardDeathTriggerService;
        }

        /// <summary>
        /// Hooks the reset flow into the resolve event emitted by the phase state machine.
        /// </summary>
        public void Initialize()
        {
            _resolveSubscription = _phaseStateMachine.OnResolve.Subscribe(HandleResolve);
        }

        public void Dispose()
        {
            _resolveSubscription?.Dispose();
        }

        private void HandleResolve(ResolveEvent _)
        {
            if (_isResetting)
            {
                return;
            }

            if (_phaseStateMachine.CurrentState != PhaseState.EndingEvaluation)
            {
                return;
            }

            _isResetting = true;
            try
            {
                ResetRunState();

                if (!_phaseStateMachine.TryTransition(PhaseState.ChoiceLock))
                {
                    return;
                }

                _phaseStateMachine.TryTransition(PhaseState.DayService);
            }
            finally
            {
                _isResetting = false;
            }
        }

        private void ResetRunState()
        {
            _soulRepository.Reset();
            _dayPhaseMechanicsService.Reset();
            _dayPhaseTimelineService.Reset();
            _dayServiceUIController.Reset();
            _ngocCotService.ResetForNight();
            _karmaHazardService.ClearHazards();
            _consequenceResolver.Reset();
            _wardDeathTriggerService.Reset();
        }
    }
}
