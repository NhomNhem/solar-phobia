using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Resources;
using SolarPhobia.Domain.ValueObjects;


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

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class DayServiceRitualIntegrationTests
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

        // ───────────────────────────────────────────────────────────
        // AC-2: Ritual Assignment via Controller
        // ───────────────────────────────────────────────────────────

        [Test]
        public void AssignRitual_DayServicePhase_Succeeds()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            bool result = _controller.AssignRitual("linh", RitualType.Tea);

            Assert.That(result, Is.True);
            Assert.That(_controller.RitualAssignments, Contains.Key("linh"));
            Assert.That(_controller.RitualAssignments["linh"], Is.EqualTo(RitualType.Tea));
        }

        [Test]
        public void AssignRitual_AbandonedSoul_Fails()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("linh");

            bool result = _controller.AssignRitual("linh", RitualType.Tea);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AssignRitual_SavedSoul_Succeeds()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");

            bool result = _controller.AssignRitual("linh", RitualType.Tea);

            Assert.That(result, Is.True);
        }

        [Test]
        public void AssignRitual_UnselectedSoul_Succeeds()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            bool result = _controller.AssignRitual("linh", RitualType.Tea);

            Assert.That(result, Is.True);
        }

        [Test]
        public void AssignRitual_InvalidSoulId_Fails()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            bool result = _controller.AssignRitual("nonexistent", RitualType.Tea);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AssignRitual_AllRitualTypes_Work()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.That(_controller.AssignRitual("linh", RitualType.Tea), Is.True);
            Assert.That(_controller.AssignRitual("van", RitualType.Incense), Is.True);
            Assert.That(_controller.AssignRitual("minh", RitualType.Offering), Is.True);
        }

        [Test]
        public void AssignRitual_WrongPhase_Fails()
        {
            _phaseMachine.SetPhase(PhaseState.Boot);

            bool result = _controller.AssignRitual("linh", RitualType.Tea);

            Assert.That(result, Is.False);
        }

        // ───────────────────────────────────────────────────────────
        // Ritual Removal via Controller
        // ───────────────────────────────────────────────────────────

        [Test]
        public void RemoveRitual_Existing_Succeeds()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.AssignRitual("linh", RitualType.Tea);

            bool removed = _controller.RemoveRitual("linh");

            Assert.That(removed, Is.True);
            Assert.That(_controller.RitualAssignments, Does.Not.ContainKey("linh"));
        }

        [Test]
        public void RemoveRitual_NoAssignment_Fails()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            bool removed = _controller.RemoveRitual("linh");

            Assert.That(removed, Is.False);
        }

        // ───────────────────────────────────────────────────────────
        // Preferred Ritual
        // ───────────────────────────────────────────────────────────

        [Test]
        public void IsPreferredRitual_LinhTea_ReturnsTrue()
        {
            bool preferred = _controller.IsPreferredRitual("linh", RitualType.Tea);

            Assert.That(preferred, Is.True);
        }

        [Test]
        public void IsPreferredRitual_LinhIncense_ReturnsFalse()
        {
            bool preferred = _controller.IsPreferredRitual("linh", RitualType.Incense);

            Assert.That(preferred, Is.False);
        }

        // ───────────────────────────────────────────────────────────
        // Confirm Flow with Ritual Assignments
        // ───────────────────────────────────────────────────────────

        [Test]
        public void ConfirmFlow_RitualsIncludedInPayload()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.AssignRitual("linh", RitualType.Tea);
            _controller.AssignRitual("van", RitualType.Incense);

            _controller.TryConfirmSelection();

            var payload = _controller.LastConfirmedPayloadValue;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.RitualAssignments, Contains.Key("linh"));
            Assert.That(payload.RitualAssignments["linh"], Is.EqualTo("Tea"));
            Assert.That(payload.RitualAssignments["van"], Is.EqualTo("Incense"));
        }

        [Test]
        public void ConfirmFlow_NoRituals_ReturnsEmptyDict()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");

            _controller.TryConfirmSelection();

            var payload = _controller.LastConfirmedPayloadValue;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.RitualAssignments, Is.Empty);
        }

        [Test]
        public void ConfirmFlow_AbandonedSoulRitual_NotInPayload()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.AssignRitual("linh", RitualType.Tea);
            _controller.AssignRitual("minh", RitualType.Offering);

            _controller.TryConfirmSelection();

            var payload = _controller.LastConfirmedPayloadValue;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.RitualAssignments, Does.Not.ContainKey("minh"));
        }

        // ───────────────────────────────────────────────────────────
        // Reset clears ritual assignments
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Reset_ClearsRitualAssignments()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.AssignRitual("linh", RitualType.Tea);

            _controller.Reset();

            Assert.That(_controller.RitualAssignments, Is.Empty);
        }

        [Test]
        public void AssignRitual_AfterConfirm_Fails()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.TryConfirmSelection();

            bool result = _controller.AssignRitual("linh", RitualType.Tea);

            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveRitual_AfterConfirm_Fails()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.ToggleSoulSelection("linh");
            _controller.ToggleSoulSelection("van");
            _controller.ToggleSoulSelection("minh");
            _controller.AssignRitual("linh", RitualType.Tea);
            _controller.TryConfirmSelection();

            bool result = _controller.RemoveRitual("linh");

            Assert.That(result, Is.False);
        }

        // ───────────────────────────────────────────────────────────
        // Phase re-entry resets rituals
        // ───────────────────────────────────────────────────────────

        [Test]
        public void PhaseReentry_ClearsRitualAssignments()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _controller.AssignRitual("linh", RitualType.Tea);

            _phaseMachine.SetPhase(PhaseState.NightTravel);
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.That(_controller.RitualAssignments, Is.Empty);
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


