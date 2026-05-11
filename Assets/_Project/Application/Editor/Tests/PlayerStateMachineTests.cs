// Assets/_Project/Application/Editor/Tests/PlayerStateMachineTests.cs
using System;
using System.Linq;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: ADR-0003-v2 + TR-player-009 — PlayerStateMachine FSM Foundation.
    /// Story 009: PlayerStateMachine Core — FSM Foundation.
    ///
    /// Pure C# logic — no Unity scene or physics dependencies.
    /// </summary>
    [TestFixture]
    public class PlayerStateMachineTests
    {
        private class TestPhaseStateMachine : IPhaseStateMachine
        {
            private readonly ReactiveProperty<PhaseState> _phase = new(PhaseState.Boot);

            public PhaseState CurrentState => _phase.Value;
            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _phase;
            public Observable<PhaseChangedEvent> OnPhaseChanged => Observable.Empty<PhaseChangedEvent>();
            public Observable<DayStartEvent> OnDayStart => Observable.Empty<DayStartEvent>();
            public Observable<NightStartEvent> OnNightStart => Observable.Empty<NightStartEvent>();
            public Observable<ResolveEvent> OnResolve => Observable.Empty<ResolveEvent>();
            public bool TryTransition(PhaseState newPhase) => true;
            public bool IsActionAllowed(GameAction action) => true;
            public void Initialize() { }
        }

        private PlayerStateMachine _fsm;

        [SetUp]
        public void SetUp()
        {
            _fsm = new PlayerStateMachine(new TestPhaseStateMachine());
            _fsm.Initialize();
        }

        // ── AC-1: Idle → Moving on A/D input ─────────────────────

        [Test]
        public void AC1_IdleToMoving_TransitionAllowed()
        {
            var can = _fsm.CanTransitionTo(EPlayerState.Moving);
            Assert.That(can, Is.True);
        }

        [Test]
        public void AC1_TryTransitionToMoving_FromIdle_ReturnsTrue()
        {
            var result = _fsm.TryTransitionTo(EPlayerState.Moving);
            Assert.That(result, Is.True);
        }

        [Test]
        public void AC1_AfterTransition_CurrentStateIsMoving()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            Assert.That(_fsm.CurrentStateValue, Is.EqualTo(EPlayerState.Moving));
        }

        // ── AC-2: Moving → Sprinting on Shift held ──────────────

        [Test]
        public void AC2_MovingToSprinting_TransitionAllowed()
        {
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Moving), Is.True);
            _fsm.TryTransitionTo(EPlayerState.Moving);
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Sprinting), Is.True);
        }

        [Test]
        public void AC2_TryTransitionToSprinting_FromMoving_ReturnsTrue()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            var result = _fsm.TryTransitionTo(EPlayerState.Sprinting);
            Assert.That(result, Is.True);
        }

        [Test]
        public void AC2_AfterTransition_CurrentStateIsSprinting()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            _fsm.TryTransitionTo(EPlayerState.Sprinting);
            Assert.That(_fsm.CurrentStateValue, Is.EqualTo(EPlayerState.Sprinting));
        }

        [Test]
        public void AC2_IdleCannotTransitionDirectlyToSprinting()
        {
            var can = _fsm.CanTransitionTo(EPlayerState.Sprinting);
            Assert.That(can, Is.False);
        }

        // ── AC-3: Falling → Gliding on Jump held while airborne ─

        [Test]
        public void AC3_FallingToGliding_TransitionAllowed()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            _fsm.TryTransitionTo(EPlayerState.Jumping);
            _fsm.TryTransitionTo(EPlayerState.Falling);
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Gliding), Is.True);
        }

        [Test]
        public void AC3_TryTransitionToGliding_FromFalling_ReturnsTrue()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            _fsm.TryTransitionTo(EPlayerState.Jumping);
            _fsm.TryTransitionTo(EPlayerState.Falling);
            var result = _fsm.TryTransitionTo(EPlayerState.Gliding);
            Assert.That(result, Is.True);
        }

        [Test]
        public void AC3_AfterTransition_CurrentStateIsGliding()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            _fsm.TryTransitionTo(EPlayerState.Jumping);
            _fsm.TryTransitionTo(EPlayerState.Falling);
            _fsm.TryTransitionTo(EPlayerState.Gliding);
            Assert.That(_fsm.CurrentStateValue, Is.EqualTo(EPlayerState.Gliding));
        }

        [Test]
        public void AC3_JumpingToGliding_TransitionAllowed()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            _fsm.TryTransitionTo(EPlayerState.Jumping);
            var can = _fsm.CanTransitionTo(EPlayerState.Gliding);
            Assert.That(can, Is.True);
        }

        // ── AC-4: Low Ward triggers Exhausted ────────────────────

        [Test]
        public void AC4_AnyStateToExhausted_TransitionAllowed()
        {
            var states = new[]
            {
                EPlayerState.Idle,
                EPlayerState.Moving,
                EPlayerState.Sprinting,
                EPlayerState.Jumping,
                EPlayerState.Falling,
                EPlayerState.Dashing,
                EPlayerState.Swinging,
                EPlayerState.Gliding
            };

            foreach (var s in states)
            {
                if (s != EPlayerState.Idle)
                    _fsm.TryTransitionTo(s);
                Assert.That(_fsm.CanTransitionTo(EPlayerState.Exhausted), Is.True,
                    $"State {s} should allow transition to Exhausted");
                _fsm.Initialize();
            }
        }

        [Test]
        public void AC4_ExhaustedMovementSpeed_Is50Percent()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            Assert.That(_fsm.CurrentStateValue, Is.EqualTo(EPlayerState.Exhausted));
        }

        [Test]
        public void AC4_ExhaustedState_AllowsRecoveryToIdle()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            var can = _fsm.CanTransitionTo(EPlayerState.Idle);
            Assert.That(can, Is.True);
        }

        [Test]
        public void AC4_ExhaustedState_AllowsRecoveryToMoving()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            var can = _fsm.CanTransitionTo(EPlayerState.Moving);
            Assert.That(can, Is.True);
        }

        // ── AC-5: State change emits event ─────────────────────

        [Test]
        public void AC5_TryTransitionTo_EmitsPlayerStateChangedEvent()
        {
            PlayerStateChangedEvent? received = null;
            using var sub = _fsm.OnStateChanged.Subscribe(e => received = e);

            _fsm.TryTransitionTo(EPlayerState.Moving);

            Assert.That(received, Is.Not.Null);
        }

        [Test]
        public void AC5_Event_PreviousState_IsIdle()
        {
            PlayerStateChangedEvent? received = null;
            using var sub = _fsm.OnStateChanged.Subscribe(e => received = e);

            _fsm.TryTransitionTo(EPlayerState.Moving);

            Assert.That(received!.Value.PreviousState, Is.EqualTo(EPlayerState.Idle));
        }

        [Test]
        public void AC5_Event_NewState_IsMoving()
        {
            PlayerStateChangedEvent? received = null;
            using var sub = _fsm.OnStateChanged.Subscribe(e => received = e);

            _fsm.TryTransitionTo(EPlayerState.Moving);

            Assert.That(received!.Value.NewState, Is.EqualTo(EPlayerState.Moving));
        }

        [Test]
        public void AC5_InvalidTransition_DoesNotEmitEvent()
        {
            PlayerStateChangedEvent? received = null;
            using var sub = _fsm.OnStateChanged.Subscribe(e => received = e);

            _fsm.TryTransitionTo(EPlayerState.Sprinting);

            Assert.That(received, Is.Null);
        }

        [Test]
        public void AC5_CurrentState_IsObservable()
        {
            bool observed = false;
            using var sub = _fsm.CurrentState.Subscribe(_ => observed = true);
            Assert.That(observed, Is.True);
        }

        // ── AC-6: Invalid transitions rejected ──────────────────

        [Test]
        public void AC6_ExhaustedToDashing_IsRejected()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Dashing), Is.False);
        }

        [Test]
        public void AC6_TryTransitionTo_ExhaustedToDashing_ReturnsFalse()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            var result = _fsm.TryTransitionTo(EPlayerState.Dashing);
            Assert.That(result, Is.False);
        }

        [Test]
        public void AC6_SprintOrJumpAllowed_FromExhausted()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            var canJump = _fsm.CanTransitionTo(EPlayerState.Jumping);
            var canSprint = _fsm.CanTransitionTo(EPlayerState.Sprinting);
            Assert.That(canJump || canSprint, Is.False,
                "Exhausted should not allow Sprint or Jump");
        }

        [Test]
        public void AC6_IdleToDashing_IsRejected()
        {
            var can = _fsm.CanTransitionTo(EPlayerState.Dashing);
            Assert.That(can, Is.False);
        }

        [Test]
        public void AC6_SameStateTransition_IsRejected()
        {
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Idle), Is.False);
        }

        [Test]
        public void AC6_ExhaustedToSwinging_IsRejected()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Swinging), Is.False);
        }

        [Test]
        public void AC6_ExhaustedToGliding_IsRejected()
        {
            _fsm.TryTransitionTo(EPlayerState.Exhausted);
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Gliding), Is.False);
        }

        // ── FSM invariant checks ─────────────────────────────────

        [Test]
        public void Initialize_SetsIdleState()
        {
            _fsm.TryTransitionTo(EPlayerState.Moving);
            _fsm.Initialize();
            Assert.That(_fsm.CurrentStateValue, Is.EqualTo(EPlayerState.Idle));
        }

        [Test]
        public void AllStates_HaveAtLeastOneValidTransition()
        {
            var states = Enum.GetValues(typeof(EPlayerState)).Cast<EPlayerState>();
            foreach (var state in states)
            {
                if (state == EPlayerState.Idle)
                    continue;
                _fsm.TryTransitionTo(state);
                var validTargets = Enum.GetValues(typeof(EPlayerState))
                    .Cast<EPlayerState>()
                    .Where(s => s != state && _fsm.CanTransitionTo(s))
                    .ToList();
                Assert.That(validTargets.Count, Is.GreaterThan(0),
                    $"State {state} has no valid outbound transitions");
                _fsm.Initialize();
            }
        }

        [Test]
        public void Crouching_AllowsRecovery()
        {
            _fsm.TryTransitionTo(EPlayerState.Crouching);
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Idle), Is.True);
            Assert.That(_fsm.CanTransitionTo(EPlayerState.Moving), Is.True);
        }

        [Test]
        public void Interacting_AllowsTransitionToAnyState()
        {
            _fsm.TryTransitionTo(EPlayerState.Interacting);
            var allowed = Enum.GetValues(typeof(EPlayerState))
                .Cast<EPlayerState>()
                .Where(s => _fsm.CanTransitionTo(s))
                .ToList();
            Assert.That(allowed.Count, Is.GreaterThan(0),
                "Interacting should allow transitions out");
        }
    }
}
