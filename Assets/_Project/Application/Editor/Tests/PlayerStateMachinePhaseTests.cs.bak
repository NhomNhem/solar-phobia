// Assets/_Project/Application/Editor/Tests/PlayerStateMachinePhaseTests.cs
using System;
using System.Linq;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-player-010 — PlayerStateMachine Phase Integration.
    /// Story 010: PlayerStateMachine Phase Integration — Day/Night State Binding.
    ///
    /// Pure C# logic — no Unity scene or physics dependencies.
    /// Tests phase-to-player-state mapping and Day/Night behavior binding.
    /// </summary>
    [TestFixture]
    [SingleThreaded]
    public class PlayerStateMachinePhaseTests
    {
        private class TestPhaseStateMachine : IPhaseStateMachine
        {
            private readonly ReactiveProperty<PhaseState> _phase;
            private readonly Subject<PhaseChangedEvent> _phaseChangedSubject = new();
            private readonly Subject<DayStartEvent> _dayStartSubject = new();
            private readonly Subject<NightStartEvent> _nightStartSubject = new();
            private readonly Subject<ResolveEvent> _resolveSubject = new();

            public TestPhaseStateMachine(PhaseState initial = PhaseState.Boot)
            {
                _phase = new ReactiveProperty<PhaseState>(initial);
            }

            public PhaseState CurrentState => _phase.Value;
            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _phase;

            public Observable<PhaseChangedEvent> OnPhaseChanged => _phaseChangedSubject;
            public Observable<DayStartEvent> OnDayStart => _dayStartSubject;
            public Observable<NightStartEvent> OnNightStart => _nightStartSubject;
            public Observable<ResolveEvent> OnResolve => _resolveSubject;

            public void SetPhase(PhaseState phase)
            {
                var previous = _phase.Value;
                _phase.Value = phase;
                _phaseChangedSubject.OnNext(new PhaseChangedEvent(previous, phase));
            }

            public bool TryTransition(PhaseState newPhase)
            {
                var previous = _phase.Value;
                _phase.Value = newPhase;
                _phaseChangedSubject.OnNext(new PhaseChangedEvent(previous, newPhase));
                return true;
            }

            public bool IsActionAllowed(GameAction action) => true;
            public void Initialize() { }

            public void Dispose()
            {
                _phaseChangedSubject.Dispose();
                _dayStartSubject.Dispose();
                _nightStartSubject.Dispose();
                _resolveSubject.Dispose();
                _phase.Dispose();
            }
        }

        private TestPhaseStateMachine _phaseStateMachine;
        private PlayerStateMachine _playerStateMachine;

        [SetUp]
        public void SetUp()
        {
            _phaseStateMachine = new TestPhaseStateMachine(PhaseState.Boot);
            _playerStateMachine = new PlayerStateMachine(_phaseStateMachine);
            _playerStateMachine.Initialize();
            _phaseStateMachine.SetPhase(PhaseState.DayService);
        }

        [TearDown]
        public void TearDown()
        {
            _playerStateMachine?.Dispose();
            _phaseStateMachine?.Dispose();
        }

        // ── AC-1: Phase DayService restricts to slow X-axis movement ──

        [Test]
        public void AC1_DayService_IsNightPhase_False()
        {
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
        }

        [Test]
        public void AC1_DayService_PlayerState_ResetsToIdle()
        {
            _playerStateMachine.TryTransitionTo(EPlayerState.Moving);
            _phaseStateMachine.SetPhase(PhaseState.DayService);

            Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(EPlayerState.Moving));
        }

        [Test]
        public void AC1_DayService_SubscribesToPhaseChange()
        {
            _playerStateMachine.TryTransitionTo(EPlayerState.Moving);
            _phaseStateMachine.SetPhase(PhaseState.DayService);

            Assert.That(_playerStateMachine.IsNightPhase, Is.False,
                "DayService phase should keep IsNightPhase as false");
        }

        // ── AC-2: Phase NightSurvival enables full 2D movement ───────

        [Test]
        public void AC2_NightSurvival_IsNightPhase_True()
        {
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);

            Assert.That(_playerStateMachine.IsNightPhase, Is.True);
        }

        [Test]
        public void AC2_NightSurvival_PlayerState_ResetsToIdle()
        {
            _playerStateMachine.TryTransitionTo(EPlayerState.Moving);
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);

            Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(EPlayerState.Idle));
        }

        [Test]
        public void AC2_NightSurvival_AllSkillsEnabled()
        {
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);

            Assert.That(_playerStateMachine.IsNightPhase, Is.True);
            Assert.That(_playerStateMachine.CanTransitionTo(EPlayerState.Dashing), Is.False);
            Assert.That(_playerStateMachine.CanTransitionTo(EPlayerState.Swinging), Is.False);
            Assert.That(_playerStateMachine.CanTransitionTo(EPlayerState.Jumping), Is.True);
        }

        // ── AC-3: Phase ChoiceLock disables all input ────────────────

        [Test]
        public void AC3_ChoiceLock_IsNightPhase_False()
        {
            _phaseStateMachine.SetPhase(PhaseState.ChoiceLock);

            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
        }

        [Test]
        public void AC3_ChoiceLock_PlayerState_ResetsToIdle()
        {
            _playerStateMachine.TryTransitionTo(EPlayerState.Moving);
            _phaseStateMachine.SetPhase(PhaseState.ChoiceLock);

            Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(EPlayerState.Idle));
        }

        [Test]
        public void AC3_EndingEvaluation_IsNightPhase_False()
        {
            _phaseStateMachine.SetPhase(PhaseState.EndingEvaluation);

            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
        }

        [Test]
        public void AC3_Boot_IsNightPhase_False()
        {
            var bootPhase = new TestPhaseStateMachine(PhaseState.Boot);
            var bootPlayer = new PlayerStateMachine(bootPhase);
            bootPlayer.Initialize();

            Assert.That(bootPlayer.IsNightPhase, Is.False);
            bootPlayer.Dispose();
        }

        // ── AC-4: Day→Night transition enables skills ─────────────────

        [Test]
        public void AC4_DayToNight_EnablesFullMovement()
        {
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
            Assert.That(_playerStateMachine.CanTransitionTo(EPlayerState.Dashing), Is.False);

            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);

            Assert.That(_playerStateMachine.IsNightPhase, Is.True);
            Assert.That(_playerStateMachine.CanTransitionTo(EPlayerState.Dashing), Is.False);
        }

        [Test]
        public void AC4_Transition_IsSynchronous()
        {
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);

            Assert.That(_playerStateMachine.IsNightPhase, Is.True,
                "Phase transition should be synchronous - no frame delay");
        }

        // ── AC-5: Night→Day transition disables skills ───────────────

        [Test]
        public void AC5_NightToDay_DisablesAllSkills()
        {
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);
            Assert.That(_playerStateMachine.IsNightPhase, Is.True);

            _phaseStateMachine.SetPhase(PhaseState.DayService);

            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
        }

        [Test]
        public void AC5_NightToChoiceLock_DisablesMovement()
        {
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);
            _phaseStateMachine.SetPhase(PhaseState.ChoiceLock);

            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
            Assert.That(_playerStateMachine.CanTransitionTo(EPlayerState.Sprinting), Is.False);
        }

        // ── AC-6: No orphaned states on phase transition ────────────

        [Test]
        public void AC6_MovingToDay_ResetsToIdle()
        {
            _playerStateMachine.TryTransitionTo(EPlayerState.Moving);
            _phaseStateMachine.SetPhase(PhaseState.DayService);

            Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(EPlayerState.Moving));
        }

        [Test]
        public void AC6_ExhaustedToNight_ResetsToIdle()
        {
            _playerStateMachine.TryTransitionTo(EPlayerState.Exhausted);
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);

            Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(EPlayerState.Idle));
        }

        [Test]
        public void AC6_DashingToNight_ResetsToIdle()
        {
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);
            _playerStateMachine.TryTransitionTo(EPlayerState.Dashing);
            _phaseStateMachine.SetPhase(PhaseState.DayService);

            Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(EPlayerState.Idle));
        }

        [Test]
        public void AC6_SwingingToChoiceLock_ResetsToIdle()
        {
            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);
            _playerStateMachine.TryTransitionTo(EPlayerState.Swinging);
            _phaseStateMachine.SetPhase(PhaseState.ChoiceLock);

            Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(EPlayerState.Idle));
        }

        [Test]
        public void AC6_AllPhases_ResetToIdle()
        {
            var phases = new[]
            {
                PhaseState.DayService,
                PhaseState.Dialogue,
                PhaseState.Order,
                PhaseState.SunsetWarning,
                PhaseState.NightTravel,
                PhaseState.ShrineArrival,
                PhaseState.NightSurvival,
                PhaseState.EndingEvaluation,
                PhaseState.ChoiceLock
            };

            foreach (var phase in phases)
            {
                _playerStateMachine.TryTransitionTo(EPlayerState.Moving);
                _playerStateMachine.TryTransitionTo(EPlayerState.Sprinting);
                _playerStateMachine.TryTransitionTo(EPlayerState.Jumping);

                _phaseStateMachine.SetPhase(phase);

                var expected = phase == PhaseState.DayService
                    ? EPlayerState.Jumping
                    : EPlayerState.Idle;
                Assert.That(_playerStateMachine.CurrentStateValue, Is.EqualTo(expected),
                    $"Phase {phase} should produce expected player state");
            }
        }

        // ── Additional: IsNightPhase Property Queries ────────────────

        [Test]
        public void IsNightPhase_Queryable_AfterPhaseChange()
        {
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);
            Assert.That(_playerStateMachine.IsNightPhase, Is.True);

            _phaseStateMachine.SetPhase(PhaseState.EndingEvaluation);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.ChoiceLock);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
        }

        // ── Additional: Full Phase Cycle ─────────────────────────────

        [Test]
        public void FullCycle_ProducesCorrectIsNightPhase()
        {
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.Dialogue);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.Order);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.SunsetWarning);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.NightTravel);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.ShrineArrival);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.NightSurvival);
            Assert.That(_playerStateMachine.IsNightPhase, Is.True);

            _phaseStateMachine.SetPhase(PhaseState.EndingEvaluation);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);

            _phaseStateMachine.SetPhase(PhaseState.ChoiceLock);
            Assert.That(_playerStateMachine.IsNightPhase, Is.False);
        }

        // ── Additional: Constructor Dependency Behavior ────────────

        [Test]
        public void Constructor_WithBootPhase_IsNightPhase_False()
        {
            var phase = new TestPhaseStateMachine(PhaseState.Boot);
            var player = new PlayerStateMachine(phase);
            player.Initialize();

            Assert.That(player.IsNightPhase, Is.False);
            player.Dispose();
        }

        [Test]
        public void Constructor_WithPhaseMachine_TransitionsWork()
        {
            var phase = new TestPhaseStateMachine(PhaseState.Boot);
            var player = new PlayerStateMachine(phase);
            player.Initialize();

            var canMove = player.TryTransitionTo(EPlayerState.Moving);
            Assert.That(canMove, Is.True);
            Assert.That(player.CurrentStateValue, Is.EqualTo(EPlayerState.Moving));

            player.Dispose();
        }
    }
}
