using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class DayServiceSelectionUITests
    {
        private StubPhaseStateMachine _phaseMachine;
        private SoulRepository _soulRepo;
        private DaySelectionValidator _validator;
        private RitualAssignmentService _ritualService;
        private DayServiceUIController _controller;

        [SetUp]
        public void SetUp()
        {
            _phaseMachine = new StubPhaseStateMachine();
            _soulRepo = new SoulRepository();
            _validator = new DaySelectionValidator();
            _ritualService = new RitualAssignmentService();
            _controller = new DayServiceUIController(_phaseMachine, _soulRepo, _validator, _ritualService);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        // ═══════════════════════════════════════════════════════════
        // AC-4: Phase Gating — UI Visibility
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void PhaseGating_DayService_ShowsUI()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.That(_controller.IsUIVisibleValue, Is.True);
        }

        [Test]
        public void PhaseGating_ChoiceLock_HidesUI()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _phaseMachine.SetPhase(PhaseState.ChoiceLock);

            Assert.That(_controller.IsUIVisibleValue, Is.False);
        }

        [Test]
        public void PhaseGating_OtherPhases_HideUI()
        {
            var hiddenPhases = new[]
            {
                PhaseState.Boot,
                PhaseState.Dialogue,
                PhaseState.Order,
                PhaseState.SunsetWarning,
                PhaseState.NightTravel,
                PhaseState.NightSurvival,
                PhaseState.EndingEvaluation
            };

            foreach (var phase in hiddenPhases)
            {
                _phaseMachine.SetPhase(phase);
                Assert.That(_controller.IsUIVisibleValue, Is.False,
                    $"UI should be hidden during {phase}");
            }
        }

        [Test]
        public void PhaseGating_ReenteringDayService_ResetsState()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");

            _phaseMachine.SetPhase(PhaseState.NightTravel);
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.That(_controller.LastValidation.SavedCount, Is.Zero);
            Assert.That(_controller.IsConfirmEnabledValue, Is.False);
        }

        // ═══════════════════════════════════════════════════════════
        // AC-3: Selection Toggle Flow
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void ToggleSelection_UnselectedToSaved()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            _controller.ToggleSoulSelection("linh");

            var selections = GetLocalSaved();
            Assert.That(selections, Does.Contain("linh"));
        }

        [Test]
        public void ToggleSelection_SavedToAbandoned()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("linh");

            var abandoned = GetLocalAbandoned();
            Assert.That(abandoned, Does.Contain("linh"));
        }

        [Test]
        public void ToggleSelection_AbandonedToUnselected()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("linh");

            var selections = GetLocalSelections();
            Assert.That(selections["linh"], Is.EqualTo(DaySelectionState.Unselected));
        }

        [Test]
        public void ToggleSelection_UnknownSoulId_DoesNothing()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.DoesNotThrow(() => _controller.ToggleSoulSelection("nonexistent"));

            Assert.That(_controller.LastValidation.SavedCount, Is.Zero);
        }

        // ═══════════════════════════════════════════════════════════
        // AC-3: Confirm Flow
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void ConfirmFlow_2Saved1Abandoned_ReturnsTrue()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            bool confirmed = _controller.TryConfirmSelection();

            Assert.That(confirmed, Is.True);
            Assert.That(_controller.IsConfirmEnabledValue, Is.False);
        }

        [Test]
        public void ConfirmFlow_SendsPayloadWithCorrectData()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            _controller.TryConfirmSelection();

            var payload = _controller.LastConfirmedPayloadValue;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.AbandonedSoulId, Is.EqualTo("minh"));
            Assert.That(payload.SavedSoulIds, Has.Count.EqualTo(2));
            Assert.That(payload.SavedSoulIds, Contains.Item("linh"));
            Assert.That(payload.SavedSoulIds, Contains.Item("van"));
        }

        [Test]
        public void ConfirmFlow_InvalidSelection_ReturnsFalse()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");

            bool confirmed = _controller.TryConfirmSelection();

            Assert.That(confirmed, Is.False);
            Assert.That(_controller.LastConfirmedPayloadValue, Is.Null);
        }

        [Test]
        public void ConfirmFlow_DoubleConfirm_SecondFails()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            _controller.TryConfirmSelection();
            bool second = _controller.TryConfirmSelection();

            Assert.That(second, Is.False);
            Assert.That(_controller.IsConfirmEnabledValue, Is.False);
        }

        // ═══════════════════════════════════════════════════════════
        // AC-7: Consequence Payload (abandoned soul ID)
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void ConsequencePayload_AbandonedSoulId_IsCorrect()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.ToggleSoulSelection("linh");

            _controller.TryConfirmSelection();

            Assert.That(_controller.LastConfirmedPayloadValue.AbandonedSoulId, Is.EqualTo("linh"));
        }

        [Test]
        public void ConsequencePayload_SavedSoulIds_AreCorrect()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.ToggleSoulSelection("linh");

            _controller.TryConfirmSelection();

            var saved = _controller.LastConfirmedPayloadValue.SavedSoulIds;
            Assert.That(saved, Has.Count.EqualTo(2));
            Assert.That(saved, Has.Member("van"));
            Assert.That(saved, Has.Member("minh"));
        }

        // ═══════════════════════════════════════════════════════════
        // AC-9: Cross-System (SoulRepository integration)
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void CrossSystem_SoulRepoUpdatedOnToggle()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            _controller.ToggleSoulSelection("linh");

            var soul = _soulRepo.GetSoul("linh");
            Assert.That(soul.DaySelection, Is.EqualTo(DaySelectionState.Saved));
        }

        [Test]
        public void CrossSystem_SoulRepoTracksAllSelections()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            Assert.That(_soulRepo.GetSoul("linh").DaySelection, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(_soulRepo.GetSoul("van").DaySelection, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(_soulRepo.GetSoul("minh").DaySelection, Is.EqualTo(DaySelectionState.Abandoned));
        }

        // ═══════════════════════════════════════════════════════════
        // AC-10: UI Feedback (Confirm button state)
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void UIFeedback_ConfirmDisabledInitially()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.That(_controller.IsConfirmEnabledValue, Is.False);
        }

        [Test]
        public void UIFeedback_ConfirmEnabledWhenSelectionValid()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            Assert.That(_controller.IsConfirmEnabledValue, Is.True);
        }

        [Test]
        public void UIFeedback_ConfirmDisabledAfterToggleToInvalid()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.ToggleSoulSelection("minh");

            Assert.That(_controller.IsConfirmEnabledValue, Is.False);
        }

        [Test]
        public void UIFeedback_ValidationStatusShowsOnInvalid()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");

            Assert.That(_controller.LastValidation, Is.Not.Null);
            Assert.That(_controller.LastValidation.IsValid, Is.False);
            Assert.That(_controller.LastValidation.ErrorMessage, Is.Not.Null);
        }

        // ═══════════════════════════════════════════════════════════
        // ChoiceLock Transition
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void ChoiceLock_Confirm_TransitionsToChoiceLock()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            _controller.TryConfirmSelection();

            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.ChoiceLock));
        }

        [Test]
        public void ChoiceLock_AfterConfirm_TogglesRejected()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            _controller.TryConfirmSelection();
            _controller.ToggleSoulSelection("linh");

            Assert.That(_controller.LastConfirmedPayloadValue, Is.Not.Null);
            Assert.That(_controller.LastConfirmedPayloadValue.AbandonedSoulId, Is.EqualTo("minh"));
        }

        // ═══════════════════════════════════════════════════════════
        // Edge Cases
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void Reset_ClearsConfirmedPayload()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.TryConfirmSelection();

            _controller.Reset();

            Assert.That(_controller.LastConfirmedPayloadValue, Is.Null);
            Assert.That(_controller.IsUIVisibleValue, Is.False);
        }

        // ═══════════════════════════════════════════════════════════
        // Helpers
        // ═══════════════════════════════════════════════════════════

        private List<string> GetLocalSaved()
        {
            var result = new List<string>();
            foreach (var id in new[] { "linh", "van", "minh" })
            {
                var soul = _soulRepo.GetSoul(id);
                if (soul != null && soul.DaySelection == DaySelectionState.Saved)
                    result.Add(id);
            }
            return result;
        }

        private Dictionary<string, DaySelectionState> GetLocalSelections()
        {
            var result = new Dictionary<string, DaySelectionState>();
            foreach (var id in new[] { "linh", "van", "minh" })
            {
                var soul = _soulRepo.GetSoul(id);
                if (soul != null)
                    result[id] = soul.DaySelection;
            }
            return result;
        }

        private List<string> GetLocalAbandoned()
        {
            var abandoned = new List<string>();
            foreach (var id in new[] { "linh", "van", "minh" })
            {
                var soul = _soulRepo.GetSoul(id);
                if (soul != null && soul.DaySelection == DaySelectionState.Abandoned)
                    abandoned.Add(id);
            }
            return abandoned;
        }

        /// <summary>
        /// Stub IPhaseStateMachine for test isolation.
        /// </summary>
        private class StubPhaseStateMachine : IPhaseStateMachine
        {
            private readonly ReactiveProperty<PhaseState> _currentPhase = new(PhaseState.Boot);
            private readonly Subject<PhaseChangedEvent> _onPhaseChanged = new();
            private readonly Subject<DayStartEvent> _onDayStart = new();
            private readonly Subject<NightStartEvent> _onNightStart = new();
            private readonly Subject<ResolveEvent> _onResolve = new();

            public PhaseState CurrentState => _currentPhase.Value;
            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _currentPhase;
            public Observable<PhaseChangedEvent> OnPhaseChanged => _onPhaseChanged;
            public Observable<DayStartEvent> OnDayStart => _onDayStart;
            public Observable<NightStartEvent> OnNightStart => _onNightStart;
            public Observable<ResolveEvent> OnResolve => _onResolve;

            public bool TryTransition(PhaseState newPhase)
            {
                var oldPhase = _currentPhase.Value;
                _currentPhase.Value = newPhase;
                _onPhaseChanged.OnNext(new PhaseChangedEvent(oldPhase, newPhase));
                return true;
            }

            public bool IsActionAllowed(GameAction action) => true;

            public void Initialize() { }

            public void SetPhase(PhaseState phase) => _currentPhase.Value = phase;
        }
    }
}
