// Assets/_Project/Application/Editor/Tests/PhaseGatedInputTests.cs
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-player-001, TR-player-008 — Phase-Gated Input.
    /// Story 001: Phase-Gated Input — Day UI / Night Movement / Disabled.
    ///
    /// Verifies that PlayerInputHandler routes to the correct PlayerInputMode
    /// for every game phase, synchronously and without frame delay.
    /// </summary>
    [TestFixture]
    public class PhaseGatedInputTests
    {
        // ── Test Helpers ───────────────────────────────────────────

        /// <summary>
        /// Minimal fake phase state machine for unit testing PlayerInputHandler in isolation.
        /// </summary>
        private class FakePhaseStateMachine : IPhaseStateMachine
        {
            private readonly ReactiveProperty<PhaseState> _phase;

            public FakePhaseStateMachine(PhaseState initial = PhaseState.Boot)
            {
                _phase = new ReactiveProperty<PhaseState>(initial);
            }

            public PhaseState CurrentState => _phase.Value;
            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _phase;

            public Observable<PhaseChangedEvent> OnPhaseChanged => new Subject<PhaseChangedEvent>();
            public Observable<DayStartEvent> OnDayStart => new Subject<DayStartEvent>();
            public Observable<NightStartEvent> OnNightStart => new Subject<NightStartEvent>();
            public Observable<ResolveEvent> OnResolve => new Subject<ResolveEvent>();

            public void SetPhase(PhaseState phase) => _phase.Value = phase;

            public bool TryTransition(PhaseState newPhase)
            {
                _phase.Value = newPhase;
                return true;
            }

            public bool IsActionAllowed(GameAction action) => true;
            public void Initialize() { }
        }

        private FakePhaseStateMachine _psm;
        private PlayerInputHandler _handler;

        [SetUp]
        public void Setup()
        {
            _psm = new FakePhaseStateMachine(PhaseState.Boot);
            _handler = new PlayerInputHandler(_psm);
            _handler.Initialize();
        }

        // ── AC-1: DayService → DayUI ───────────────────────────────

        [Test]
        public void AC1_DayService_SetsMode_DayUI()
        {
            _psm.SetPhase(PhaseState.DayService);

            Assert.AreEqual(PlayerInputMode.DayUI, _handler.CurrentMode.CurrentValue);
        }

        [Test]
        public void AC1_DayService_IsUIEnabled_True()
        {
            _psm.SetPhase(PhaseState.DayService);

            Assert.IsTrue(_handler.IsUIEnabled);
        }

        [Test]
        public void AC1_DayService_IsMovementEnabled_False()
        {
            _psm.SetPhase(PhaseState.DayService);

            Assert.IsFalse(_handler.IsMovementEnabled);
        }

        // ── AC-2: NightSurvival → NightMovement ───────────────────

        [Test]
        public void AC2_NightSurvival_SetsMode_NightMovement()
        {
            _psm.SetPhase(PhaseState.NightSurvival);

            Assert.AreEqual(PlayerInputMode.NightMovement, _handler.CurrentMode.CurrentValue);
        }

        [Test]
        public void AC2_NightSurvival_IsMovementEnabled_True()
        {
            _psm.SetPhase(PhaseState.NightSurvival);

            Assert.IsTrue(_handler.IsMovementEnabled);
        }

        [Test]
        public void AC2_NightSurvival_IsUIEnabled_False()
        {
            _psm.SetPhase(PhaseState.NightSurvival);

            Assert.IsFalse(_handler.IsUIEnabled);
        }

        // ── AC-3: ChoiceLock → Disabled ───────────────────────────

        [Test]
        public void AC3_ChoiceLock_SetsMode_Disabled()
        {
            _psm.SetPhase(PhaseState.ChoiceLock);

            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);
        }

        [Test]
        public void AC3_EndingEvaluation_SetsMode_Disabled()
        {
            _psm.SetPhase(PhaseState.EndingEvaluation);

            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);
        }

        [Test]
        public void AC3_Boot_SetsMode_Disabled()
        {
            // Initial state from Setup
            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);
        }

        // ── AC-4: Clean exit from NightSurvival ───────────────────

        [Test]
        public void AC4_ExitNightSurvival_ToEndingEvaluation_SetsDisabled()
        {
            _psm.SetPhase(PhaseState.NightSurvival);
            Assert.AreEqual(PlayerInputMode.NightMovement, _handler.CurrentMode.CurrentValue);

            _psm.SetPhase(PhaseState.EndingEvaluation);

            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);
            Assert.IsFalse(_handler.IsMovementEnabled, "Movement must be disabled after NightSurvival exits");
            Assert.IsFalse(_handler.IsUIEnabled, "UI must not activate on EndingEvaluation");
        }

        [Test]
        public void AC4_ExitNightSurvival_NoOrphanedMovementState()
        {
            _psm.SetPhase(PhaseState.NightSurvival);
            _psm.SetPhase(PhaseState.EndingEvaluation);
            _psm.SetPhase(PhaseState.ChoiceLock);

            Assert.IsFalse(_handler.IsMovementEnabled);
            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);
        }

        // ── AC-5: No combat inputs (structural — mode never enables combat) ──

        [Test]
        public void AC5_NoCombatMode_ExistsInEnum()
        {
            // PlayerInputMode must not contain a Combat or Attack variant
            var values = System.Enum.GetValues(typeof(PlayerInputMode));
            foreach (PlayerInputMode mode in values)
            {
                string name = mode.ToString();
                Assert.IsFalse(
                    name.Contains("Combat") || name.Contains("Attack") || name.Contains("Block"),
                    $"Combat input mode '{name}' must not exist in PlayerInputMode enum"
                );
            }
        }

        // ── Mode Transition Sequence ───────────────────────────────

        [Test]
        public void ModeTransition_FullCycle_CorrectSequence()
        {
            // Boot → DayService → NightSurvival → EndingEvaluation → ChoiceLock → DayService
            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);

            _psm.SetPhase(PhaseState.DayService);
            Assert.AreEqual(PlayerInputMode.DayUI, _handler.CurrentMode.CurrentValue);

            _psm.SetPhase(PhaseState.NightSurvival);
            Assert.AreEqual(PlayerInputMode.NightMovement, _handler.CurrentMode.CurrentValue);

            _psm.SetPhase(PhaseState.EndingEvaluation);
            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);

            _psm.SetPhase(PhaseState.ChoiceLock);
            Assert.AreEqual(PlayerInputMode.Disabled, _handler.CurrentMode.CurrentValue);

            _psm.SetPhase(PhaseState.DayService);
            Assert.AreEqual(PlayerInputMode.DayUI, _handler.CurrentMode.CurrentValue);
        }

        [Test]
        public void ModeTransition_IsSynchronous_NoFrameDelay()
        {
            // Mode must update in the same call — no deferred/async update
            _psm.SetPhase(PhaseState.NightSurvival);

            // Immediately after SetPhase — no yield, no tick required
            Assert.AreEqual(PlayerInputMode.NightMovement, _handler.CurrentMode.CurrentValue);
        }

        // ── ReactiveProperty Emission ──────────────────────────────

        [Test]
        public void CurrentMode_EmitsEvent_OnPhaseChange()
        {
            var emittedModes = new System.Collections.Generic.List<PlayerInputMode>();
            using var sub = _handler.CurrentMode.Subscribe(m => emittedModes.Add(m));

            _psm.SetPhase(PhaseState.DayService);
            _psm.SetPhase(PhaseState.NightSurvival);

            // Initial value (Disabled from Boot) + DayUI + NightMovement = 3 emissions
            Assert.GreaterOrEqual(emittedModes.Count, 2);
            Assert.AreEqual(PlayerInputMode.DayUI, emittedModes[emittedModes.Count - 2]);
            Assert.AreEqual(PlayerInputMode.NightMovement, emittedModes[emittedModes.Count - 1]);
        }

        [Test]
        public void CurrentMode_DoesNotEmit_WhenModeUnchanged()
        {
            _psm.SetPhase(PhaseState.ChoiceLock); // Disabled
            int emitCount = 0;
            using var sub = _handler.CurrentMode.Subscribe(_ => emitCount++);
            int baseline = emitCount;

            _psm.SetPhase(PhaseState.EndingEvaluation); // Also Disabled — no change

            // ReactiveProperty only emits on value change
            Assert.AreEqual(baseline, emitCount, "No emission when mode stays Disabled");
        }

        // ── Travel / Intermediate Phases → Disabled ───────────────

        [Test]
        public void IntermediatePhases_AllMapTo_Disabled()
        {
            var intermediatePhases = new[]
            {
                PhaseState.Dialogue,
                PhaseState.Order,
                PhaseState.SunsetWarning,
                PhaseState.NightTravel,
                PhaseState.ShrineArrival
            };

            foreach (var phase in intermediatePhases)
            {
                _psm.SetPhase(phase);
                Assert.AreEqual(
                    PlayerInputMode.Disabled,
                    _handler.CurrentMode.CurrentValue,
                    $"Phase {phase} should map to Disabled"
                );
            }
        }
    }
}
