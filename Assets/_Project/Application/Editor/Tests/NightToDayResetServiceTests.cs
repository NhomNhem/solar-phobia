using System;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Services;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Application.Services.Objective;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using PhaseDayPhaseMechanicsService = SolarPhobia.Application.Phase.Day.DayPhaseMechanicsService;
using PhaseDayPhaseTimelineService = SolarPhobia.Application.Phase.Timeline.DayPhaseTimelineService;
using NightToDayResetService = SolarPhobia.Application.Phase.Reset.NightToDayResetService;
using ApplicationWardTimerService = SolarPhobia.Application.Services.IWardTimerService;
using PhaseWardTimerService = SolarPhobia.Infrastructure.Services.WardTimerService;
using NgocCotService = SolarPhobia.Application.Services.NgocCotService;
using RitualAssignmentService = SolarPhobia.Application.Services.RitualAssignmentService;
using WardDeathTriggerService = SolarPhobia.Application.Services.Objective.WardDeathTriggerService;

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class NightToDayResetServiceTests
    {
        private PhaseStateMachine _phaseMachine;
        private SoulRepository _soulRepository;
        private PhaseDayPhaseMechanicsService _dayPhaseMechanicsService;
        private PhaseDayPhaseTimelineService _dayPhaseTimelineService;
        private DayServiceUIController _dayServiceUIController;
        private NgocCotService _ngocCotService;
        private SpyKarmaHazardService _karmaHazardService;
        private SpyConsequenceResolver _consequenceResolver;
        private SpyWardDeathTriggerService _wardDeathTriggerService;
        private NightToDayResetService _resetService;

        [SetUp]
        public void SetUp()
        {
            _phaseMachine = new PhaseStateMachine();
            _phaseMachine.Initialize();

            _soulRepository = new SoulRepository();
            _dayPhaseMechanicsService = new PhaseDayPhaseMechanicsService(_soulRepository, new NoOpAnimationService(), new NoOpAudioService());
            _dayPhaseTimelineService = new PhaseDayPhaseTimelineService();
            _dayServiceUIController = new DayServiceUIController(_phaseMachine, _soulRepository, new DaySelectionValidator(), new RitualAssignmentService());
            _ngocCotService = new NgocCotService();
            _karmaHazardService = new SpyKarmaHazardService();
            _consequenceResolver = new SpyConsequenceResolver();
            _wardDeathTriggerService = new SpyWardDeathTriggerService();

            _resetService = new NightToDayResetService(
                _phaseMachine,
                _soulRepository,
                _dayPhaseMechanicsService,
                _dayPhaseTimelineService,
                _dayServiceUIController,
                _ngocCotService,
                _karmaHazardService,
                _consequenceResolver,
                _wardDeathTriggerService);
            _resetService.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _resetService?.Dispose();
            _dayServiceUIController?.Dispose();
        }

        [Test]
        public void Resolve_TransitionsBackToDayServiceAndClearsRunState()
        {
            // Dirty run-scoped state before the night ends.
            _soulRepository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            _soulRepository.TrySetSelection("van", DaySelectionState.Abandoned, PhaseState.DayService);
            _dayPhaseMechanicsService.TryShove("van", PhaseState.DayService);
            _dayPhaseTimelineService.StartTimeline();
            _dayPhaseTimelineService.Tick(120f);
            _dayServiceUIController.ToggleSoulSelection("linh");
            _dayServiceUIController.AssignRitual("linh", RitualType.Tea);
            _ngocCotService.TryCollectRelic();
            _ngocCotService.TryCollectRelic();
            _consequenceResolver.Resolve("linh");
            _wardDeathTriggerService.MarkTriggered();
            _karmaHazardService.SpawnHazardForGhost("Van", Vector3.zero);

            GoToNightSurvival();

            Assert.That(_phaseMachine.TryTransition(PhaseState.EndingEvaluation), Is.True);

            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.DayService));
            Assert.That(_soulRepository.GetSoul("linh").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(_soulRepository.GetSoul("van").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(_dayPhaseMechanicsService.GetSacrificedGhostId(), Is.Null);
            Assert.That(_dayPhaseTimelineService.ElapsedTime, Is.EqualTo(0f));
            Assert.That(_dayPhaseTimelineService.CurrentPhase, Is.EqualTo(TimelinePhase.Stability));
            Assert.That(_dayServiceUIController.IsUIVisibleValue, Is.True);
            Assert.That(_dayServiceUIController.IsConfirmEnabledValue, Is.False);
            Assert.That(_dayServiceUIController.RitualAssignments.Count, Is.EqualTo(0));
            Assert.That(_ngocCotService.BoneCount, Is.EqualTo(0));
            Assert.That(_karmaHazardService.ClearCount, Is.EqualTo(1));
            Assert.That(_consequenceResolver.ResetCount, Is.EqualTo(1));
            Assert.That(_consequenceResolver.HasResolved, Is.False);
            Assert.That(_wardDeathTriggerService.ResetCount, Is.EqualTo(1));
            Assert.That(_wardDeathTriggerService.HasTriggeredDeath, Is.False);
        }

        [Test]
        public void Resolve_RealWardDepletion_TransitionsBackToDayService()
        {
            using var harness = new RealResetHarness();
            harness.DirtyDayState();
            harness.GoToNightSurvival();

            ((ApplicationWardTimerService)harness.WardTimerService).CurrentWard = 0f;

            Assert.That(harness.PhaseMachine.CurrentState, Is.EqualTo(PhaseState.DayService));
            Assert.That(harness.SoulRepository.GetSoul("linh").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(harness.SoulRepository.GetSoul("van").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(harness.DayPhaseMechanicsService.GetSacrificedGhostId(), Is.Null);
            Assert.That(harness.DayPhaseTimelineService.ElapsedTime, Is.EqualTo(0f));
            Assert.That(harness.DayPhaseTimelineService.CurrentPhase, Is.EqualTo(TimelinePhase.Stability));
            Assert.That(harness.DayServiceUIController.IsUIVisibleValue, Is.True);
            Assert.That(harness.DayServiceUIController.IsConfirmEnabledValue, Is.False);
            Assert.That(harness.DayServiceUIController.RitualAssignments, Is.Empty);
            Assert.That(harness.NgocCotService.BoneCount, Is.EqualTo(0));
            Assert.That(harness.KarmaHazardService.ClearCount, Is.EqualTo(1));
            Assert.That(harness.ConsequenceResolver.ResetCount, Is.EqualTo(1));
            Assert.That(harness.WardDeathTriggerService.HasTriggeredDeath, Is.False);
        }

        [Test]
        public void Resolve_RealShrineArrival_TransitionsBackToDayService()
        {
            using var harness = new RealResetHarness();
            harness.DirtyDayState();
            harness.GoToNightSurvival();

            var shrineService = new ShrineObjectiveService(harness.PhaseMachine);

            Assert.That(shrineService.TryTriggerShrineArrival(1f), Is.True);
            Assert.That(harness.PhaseMachine.CurrentState, Is.EqualTo(PhaseState.DayService));
            Assert.That(harness.SoulRepository.GetSoul("linh").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(harness.SoulRepository.GetSoul("van").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(harness.DayPhaseMechanicsService.GetSacrificedGhostId(), Is.Null);
            Assert.That(harness.DayPhaseTimelineService.ElapsedTime, Is.EqualTo(0f));
            Assert.That(harness.DayPhaseTimelineService.CurrentPhase, Is.EqualTo(TimelinePhase.Stability));
            Assert.That(harness.DayServiceUIController.IsUIVisibleValue, Is.True);
            Assert.That(harness.DayServiceUIController.IsConfirmEnabledValue, Is.False);
            Assert.That(harness.DayServiceUIController.RitualAssignments, Is.Empty);
            Assert.That(harness.NgocCotService.BoneCount, Is.EqualTo(0));
            Assert.That(harness.KarmaHazardService.ClearCount, Is.EqualTo(1));
            Assert.That(harness.ConsequenceResolver.ResetCount, Is.EqualTo(1));
            Assert.That(harness.WardDeathTriggerService.HasTriggeredDeath, Is.False);
        }

        private void GoToNightSurvival()
        {
            Assert.That(_phaseMachine.TryTransition(PhaseState.Dialogue), Is.True);
            Assert.That(_phaseMachine.TryTransition(PhaseState.Order), Is.True);
            Assert.That(_phaseMachine.TryTransition(PhaseState.SunsetWarning), Is.True);
            Assert.That(_phaseMachine.TryTransition(PhaseState.NightTravel), Is.True);
            Assert.That(_phaseMachine.TryTransition(PhaseState.ShrineArrival), Is.True);
            Assert.That(_phaseMachine.TryTransition(PhaseState.NightSurvival), Is.True);
        }

        private sealed class NoOpAudioService : IAudioService
        {
            public void PlayDashSound() { }
            public void PlayShoveImpact() { }
            public void PlaySoulBurn() { }
            public void PlaySprintSound() { }
            public void PlaySwapSound() { }
            public void PlaySwingSound() { }
        }

        private sealed class NoOpAnimationService : IAnimationService
        {
            public void PlayShoveAnimation(string playerId, string soulId) { }
            public void PlaySwapAnimation(string playerId, string soulId, float duration) { }
        }

        private sealed class SpyKarmaHazardService : IKarmaHazardService
        {
            public int ClearCount { get; private set; }

            public Observable<KarmaHazardData> OnHazardSpawned => Observable.Empty<KarmaHazardData>();

            public void ClearHazards()
            {
                ClearCount++;
            }

            public void SpawnHazardForGhost(string ghostType, Vector3 position)
            {
            }
        }

        private sealed class SpyConsequenceResolver : IConsequenceResolver
        {
            public bool HasResolved { get; private set; }
            public int ResetCount { get; private set; }

            public CursePayload Resolve(string abandonedSoulId)
            {
                HasResolved = true;
                return new CursePayload
                {
                    CurseType = NightOutcomeState.Drag,
                    Intensity = 1.0f,
                    SpawnBias = abandonedSoulId
                };
            }

            public void Reset()
            {
                ResetCount++;
                HasResolved = false;
            }
        }

        private sealed class SpyWardDeathTriggerService : IWardDeathTriggerService
        {
            public bool HasTriggeredDeath { get; private set; }
            public int ResetCount { get; private set; }

            public void Dispose()
            {
            }

            public void MarkTriggered()
            {
                HasTriggeredDeath = true;
            }

            public void Reset()
            {
                ResetCount++;
                HasTriggeredDeath = false;
            }
        }

        private sealed class RealResetHarness : IDisposable
        {
            public PhaseStateMachine PhaseMachine { get; }
            public SoulRepository SoulRepository { get; }
            public PhaseDayPhaseMechanicsService DayPhaseMechanicsService { get; }
            public PhaseDayPhaseTimelineService DayPhaseTimelineService { get; }
            public DayServiceUIController DayServiceUIController { get; }
            public NgocCotService NgocCotService { get; }
            public SpyKarmaHazardService KarmaHazardService { get; }
            public SpyConsequenceResolver ConsequenceResolver { get; }
            public PhaseWardTimerService WardTimerService { get; }
            public WardDeathTriggerService WardDeathTriggerService { get; }
            public NightToDayResetService ResetService { get; }

            public RealResetHarness()
            {
                PhaseMachine = new PhaseStateMachine();
                PhaseMachine.Initialize();

                SoulRepository = new SoulRepository();
                DayPhaseMechanicsService = new PhaseDayPhaseMechanicsService(
                    SoulRepository,
                    new NoOpAnimationService(),
                    new NoOpAudioService());
                DayPhaseTimelineService = new PhaseDayPhaseTimelineService();
                DayServiceUIController = new DayServiceUIController(
                    PhaseMachine,
                    SoulRepository,
                    new DaySelectionValidator(),
                    new RitualAssignmentService());
                NgocCotService = new NgocCotService();
                KarmaHazardService = new SpyKarmaHazardService();
                ConsequenceResolver = new SpyConsequenceResolver();
                WardTimerService = new PhaseWardTimerService(PhaseMachine);
                WardDeathTriggerService = new WardDeathTriggerService(WardTimerService, PhaseMachine);
                ResetService = new NightToDayResetService(
                    PhaseMachine,
                    SoulRepository,
                    DayPhaseMechanicsService,
                    DayPhaseTimelineService,
                    DayServiceUIController,
                    NgocCotService,
                    KarmaHazardService,
                    ConsequenceResolver,
                    WardDeathTriggerService);

                WardDeathTriggerService.Initialize();
                ResetService.Initialize();
            }

            public void DirtyDayState()
            {
                SoulRepository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
                SoulRepository.TrySetSelection("van", DaySelectionState.Abandoned, PhaseState.DayService);
                DayPhaseMechanicsService.TryShove("van", PhaseState.DayService);
                DayPhaseTimelineService.StartTimeline();
                DayPhaseTimelineService.Tick(120f);
                DayServiceUIController.ToggleSoulSelection("linh");
                DayServiceUIController.AssignRitual("linh", RitualType.Tea);
                NgocCotService.TryCollectRelic();
                NgocCotService.TryCollectRelic();
                ConsequenceResolver.Resolve("linh");
                KarmaHazardService.SpawnHazardForGhost("Van", Vector3.zero);
                ((ApplicationWardTimerService)WardTimerService).CurrentWard = 10f;
            }

            public void GoToNightSurvival()
            {
                Assert.That(PhaseMachine.TryTransition(PhaseState.Dialogue), Is.True);
                Assert.That(PhaseMachine.TryTransition(PhaseState.Order), Is.True);
                Assert.That(PhaseMachine.TryTransition(PhaseState.SunsetWarning), Is.True);
                Assert.That(PhaseMachine.TryTransition(PhaseState.NightTravel), Is.True);
                Assert.That(PhaseMachine.TryTransition(PhaseState.ShrineArrival), Is.True);
                Assert.That(PhaseMachine.TryTransition(PhaseState.NightSurvival), Is.True);
            }

            public void Dispose()
            {
                ResetService.Dispose();
                WardDeathTriggerService.Dispose();
                DayServiceUIController.Dispose();
            }
        }
    }
}
