using System.Linq;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Features.Phase.Flow;
using SolarPhobia.Application.Features.Phase.Reset;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Application.Features.Resources;
using SolarPhobia.Application.Features.Strike;
using SolarPhobia.Application.Features.Consequences.WaterTrap;
using SolarPhobia.Application.Features.Rituals;
using SolarPhobia.Application.Features.Shrines;
using SolarPhobia.Application.Features.Day;
using SolarPhobia.Application.Features.Player.State;
using SolarPhobia.Application.Features.Player.Input;
using SolarPhobia.Application.Features.Player.Interactions;
using SolarPhobia.Application.Features.Player.Cursor;
using SolarPhobia.Application.Features.Player.Events;
using SolarPhobia.Application.Features.Combat;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Domain.Events;
using SolarPhobia.Infrastructure.Features.Soul;
using UnityEngine;

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class SoulRepositoryTests
    {
        private SoulRepository _repository;
        private PhaseStateMachine _phaseMachine;

        [SetUp]
        public void SetUp()
        {
            _phaseMachine = new PhaseStateMachine();
            _phaseMachine.Initialize();
            _repository = new SoulRepository();
        }

        [Test]
        public void Initialize_CreatesThreeSouls()
        {
            Assert.That(_repository.Souls.Count, Is.EqualTo(3));
            Assert.That(_repository.GetSoul("linh"), Is.Not.Null);
            Assert.That(_repository.GetSoul("van"), Is.Not.Null);
            Assert.That(_repository.GetSoul("minh"), Is.Not.Null);
        }

        [Test]
        public void GetSoul_ReturnsCorrectSoul()
        {
            var linh = _repository.GetSoul("linh");
            Assert.That(linh.LocalizedName, Is.EqualTo("Em Linh"));
        }

        [Test]
        public void GetSoul_UnknownId_ReturnsNull()
        {
            var result = _repository.GetSoul("unknown");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void TrySetSelection_InDayService_Succeeds()
        {
            var result = _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            Assert.That(result, Is.True);
            Assert.That(_repository.GetSoul("linh").DaySelection, Is.EqualTo(DaySelectionState.Saved));
        }

        [Test]
        public void TrySetSelection_OutsideDayService_Fails()
        {
            var result = _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.ChoiceLock);
            Assert.That(result, Is.False);
        }

        [Test]
        public void TrySetSelection_EmitsSelectionChangedEvent()
        {
            SelectionChangedEvent? received = null;
            var sub = _repository.OnSelectionChanged.Subscribe(e => received = e);

            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);

            Assert.That(received, Is.Not.Null);
            Assert.That(received.Value.SoulId, Is.EqualTo("linh"));
            Assert.That(received.Value.OldState, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(received.Value.NewState, Is.EqualTo(DaySelectionState.Saved));
            sub.Dispose();
        }

        [Test]
        public void TrySetSelection_AbandonedThenSaved_Fails()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Abandoned, PhaseState.DayService);

            var result = _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetSavedSouls_ReturnsOnlySavedSouls()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("van", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("minh", DaySelectionState.Abandoned, PhaseState.DayService);

            var saved = _repository.GetSavedSouls();
            Assert.That(saved.Count, Is.EqualTo(2));
            Assert.That(saved.Any(s => s.Id == "linh"), Is.True);
            Assert.That(saved.Any(s => s.Id == "van"), Is.True);
        }

        [Test]
        public void GetAbandonedSoul_ReturnsOnlyAbandonedSouls()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("van", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("minh", DaySelectionState.Abandoned, PhaseState.DayService);

            var abandoned = _repository.GetAbandonedSoul();
            Assert.That(abandoned.Count, Is.EqualTo(1));
            Assert.That(abandoned[0].Id, Is.EqualTo("minh"));
        }

        [Test]
        public void IsSelectionValid_TwoSavedOneAbandoned_ReturnsTrue()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("van", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("minh", DaySelectionState.Abandoned, PhaseState.DayService);

            var result = _repository.IsSelectionValid(2);
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSelectionValid_WrongCount_ReturnsFalse()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("van", DaySelectionState.Abandoned, PhaseState.DayService);
            _repository.TrySetSelection("minh", DaySelectionState.Abandoned, PhaseState.DayService);

            var result = _repository.IsSelectionValid(2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void TrySetNightOutcome_InChoiceLock_Succeeds()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Abandoned, PhaseState.DayService);

            var result = _repository.TrySetNightOutcome("linh", NightOutcomeState.Drag, PhaseState.ChoiceLock);
            Assert.That(result, Is.True);
            Assert.That(_repository.GetSoul("linh").NightOutcome, Is.EqualTo(NightOutcomeState.Drag));
        }

        [Test]
        public void TrySetNightOutcome_SavedSoul_Fails()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);

            var result = _repository.TrySetNightOutcome("linh", NightOutcomeState.Drag, PhaseState.ChoiceLock);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Reset_ClearsAllSelections()
        {
            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("van", DaySelectionState.Saved, PhaseState.DayService);
            _repository.TrySetSelection("minh", DaySelectionState.Abandoned, PhaseState.DayService);

            _repository.Reset();

            Assert.That(_repository.GetSoul("linh").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(_repository.GetSoul("van").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
            Assert.That(_repository.GetSoul("minh").DaySelection, Is.EqualTo(DaySelectionState.Unselected));
        }

        [Test]
        public void Reset_ClearsNightOutcomes()
        {
            _repository.TrySetSelection("minh", DaySelectionState.Abandoned, PhaseState.DayService);
            _repository.TrySetNightOutcome("minh", NightOutcomeState.FakeShrine, PhaseState.ChoiceLock);

            _repository.Reset();

            Assert.That(_repository.GetSoul("minh").NightOutcome, Is.EqualTo(NightOutcomeState.None));
        }

        [Test]
        public void OnSelectionChanged_IsObservable()
        {
            bool observed = false;
            var sub = _repository.OnSelectionChanged.Subscribe(_ => observed = true);
            _repository.TrySetSelection("linh", DaySelectionState.Saved, PhaseState.DayService);
            Assert.That(observed, Is.True);
            sub.Dispose();
        }
    }
}






