using System.Linq;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class PhaseStateMachineTests
    {
        private PhaseStateMachine _machine;

        [SetUp]
        public void SetUp()
        {
            _machine = new PhaseStateMachine();
            _machine.Initialize();
        }

        [Test]
        public void Initialize_StartsInDayService()
        {
            Assert.That(_machine.CurrentState, Is.EqualTo(PhaseState.DayService));
        }

        [Test]
        public void CurrentPhase_IsObservable()
        {
            bool observed = false;
            var sub = _machine.CurrentPhase.Subscribe(_ => observed = true);
            Assert.That(observed, Is.True);
            sub.Dispose();
        }

        [Test]
        public void TryTransition_FromDayServiceToDialogue_ReturnsTrue()
        {
            var result = _machine.TryTransition(PhaseState.Dialogue);
            Assert.That(result, Is.True);
            Assert.That(_machine.CurrentState, Is.EqualTo(PhaseState.Dialogue));
        }

        [Test]
        public void TryTransition_InvalidTransition_ReturnsFalse()
        {
            var result = _machine.TryTransition(PhaseState.NightSurvival);
            Assert.That(result, Is.False);
            Assert.That(_machine.CurrentState, Is.EqualTo(PhaseState.DayService));
        }

        [Test]
        public void TryTransition_EmitsPhaseChangedEvent()
        {
            PhaseChangedEvent? received = null;
            var sub = _machine.OnPhaseChanged.Subscribe(e => received = e);
            
            _machine.TryTransition(PhaseState.Dialogue);
            
            Assert.That(received, Is.Not.Null);
            Assert.That(received.Value.PreviousPhase, Is.EqualTo(PhaseState.DayService));
            Assert.That(received.Value.NewPhase, Is.EqualTo(PhaseState.Dialogue));
            sub.Dispose();
        }

        [Test]
        public void TryTransition_ToNightSurvival_EmitsNightStartEvent()
        {
            NightStartEvent? received = null;
            var sub = _machine.OnNightStart.Subscribe(e => received = e);
            
            // Full path: DayService -> Dialogue -> Order -> SunsetWarning -> NightTravel -> ShrineArrival -> NightSurvival
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            _machine.TryTransition(PhaseState.NightSurvival);
            
            Assert.That(received, Is.Not.Null);
            sub.Dispose();
        }

        [Test]
        public void TryTransition_ToEndingEvaluation_EmitsResolveEvent()
        {
            ResolveEvent? received = null;
            var sub = _machine.OnResolve.Subscribe(e => received = e);
            
            // Full path to NightSurvival then to EndingEvaluation
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            _machine.TryTransition(PhaseState.NightSurvival);
            _machine.TryTransition(PhaseState.EndingEvaluation);
            
            Assert.That(received, Is.Not.Null);
            sub.Dispose();
        }

        [Test]
        public void IsActionAllowed_DayService_ReturnsTrueForDayActions()
        {
            Assert.That(_machine.IsActionAllowed(GameAction.StartGame), Is.True);
            Assert.That(_machine.IsActionAllowed(GameAction.CompleteDayPrep), Is.True);
            Assert.That(_machine.IsActionAllowed(GameAction.FinishDialogue), Is.True);
        }

        [Test]
        public void IsActionAllowed_DayService_ReturnsFalseForTravelActions()
        {
            Assert.That(_machine.IsActionAllowed(GameAction.ArriveAtShrine), Is.False);
            Assert.That(_machine.IsActionAllowed(GameAction.SurviveNight), Is.False);
        }

        [Test]
        public void IsActionAllowed_ShrineArrival_OnlyAllowsMakeChoice()
        {
            // Go to ShrineArrival
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            
            Assert.That(_machine.IsActionAllowed(GameAction.MakeChoice), Is.True);
            Assert.That(_machine.IsActionAllowed(GameAction.CompleteDayPrep), Is.False);
        }

        [Test]
        public void IsActionAllowed_NightSurvival_ReturnsTrueForSurvivalActions()
        {
            // Go to NightSurvival
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            _machine.TryTransition(PhaseState.NightSurvival);
            
            Assert.That(_machine.IsActionAllowed(GameAction.ResetGame), Is.True);
            Assert.That(_machine.IsActionAllowed(GameAction.StartGame), Is.False);
        }

        [Test]
        public void IsActionAllowed_EndingEvaluation_ReturnsFalseForAll()
        {
            // Go to EndingEvaluation
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            _machine.TryTransition(PhaseState.NightSurvival);
            _machine.TryTransition(PhaseState.EndingEvaluation);
            
            Assert.That(_machine.IsActionAllowed(GameAction.ResetGame), Is.False);
            Assert.That(_machine.IsActionAllowed(GameAction.StartGame), Is.False);
        }

        [Test]
        public void TryTransition_InvalidFromEndingEvaluation_Blocked()
        {
            // Go to EndingEvaluation
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            _machine.TryTransition(PhaseState.NightSurvival);
            _machine.TryTransition(PhaseState.EndingEvaluation);
            
            var result = _machine.TryTransition(PhaseState.DayService);
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryTransition_ChoiceLock_FromEndingEvaluation()
        {
            // Go to EndingEvaluation
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            _machine.TryTransition(PhaseState.NightSurvival);
            _machine.TryTransition(PhaseState.EndingEvaluation);
            
            var result = _machine.TryTransition(PhaseState.ChoiceLock);
            
            Assert.That(result, Is.True);
            Assert.That(_machine.CurrentState, Is.EqualTo(PhaseState.ChoiceLock));
        }

        [Test]
        public void TryTransition_CanGoBackToDayService_FromChoiceLock()
        {
            // Go to EndingEvaluation then ChoiceLock
            _machine.TryTransition(PhaseState.Dialogue);
            _machine.TryTransition(PhaseState.Order);
            _machine.TryTransition(PhaseState.SunsetWarning);
            _machine.TryTransition(PhaseState.NightTravel);
            _machine.TryTransition(PhaseState.ShrineArrival);
            _machine.TryTransition(PhaseState.NightSurvival);
            _machine.TryTransition(PhaseState.EndingEvaluation);
            _machine.TryTransition(PhaseState.ChoiceLock);
            
            var result = _machine.TryTransition(PhaseState.DayService);
            
            Assert.That(result, Is.True);
            Assert.That(_machine.CurrentState, Is.EqualTo(PhaseState.DayService));
        }
    }
}