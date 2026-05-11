using System;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Unit tests for DayPhaseTimelineService.
    /// Tests timeline phase transitions and capacity changes.
    /// </summary>
    [TestFixture]
    public class DayPhaseTimelineTests
    {
        private DayPhaseTimelineService _timeline;

        [SetUp]
        public void SetUp()
        {
            _timeline = new DayPhaseTimelineService();
        }

        // ── Initial State Tests ─────────────────────────────────────────────────
        
        [Test]
        public void StartTimeline_BeginsAtStability()
        {
            _timeline.StartTimeline();
            
            Assert.That(_timeline.CurrentPhase, Is.EqualTo(TimelinePhase.Stability));
        }

        [Test]
        public void ElapsedTime_StartsAtZero()
        {
            _timeline.StartTimeline();
            
            Assert.That(_timeline.ElapsedTime, Is.EqualTo(0f));
        }

        [Test]
        public void CurrentTimelinePhase_IsObservable()
        {
            bool observed = false;
            var sub = _timeline.CurrentTimelinePhase.Subscribe(_ => observed = true);
            
            Assert.That(observed, Is.True);
            sub.Dispose();
        }

        // ── Phase Transition Tests ─────────────────────────────────────────────

        [Test]
        public void PhaseTransition_StabilityToTension_At90Seconds()
        {
            _timeline.StartTimeline();
            _timeline.Tick(90f);
            
            Assert.That(_timeline.CurrentPhase, Is.EqualTo(TimelinePhase.Tension));
        }

        [Test]
        public void PhaseTransition_TensionToCrisis_At180Seconds()
        {
            _timeline.StartTimeline();
            _timeline.Tick(180f);
            
            Assert.That(_timeline.CurrentPhase, Is.EqualTo(TimelinePhase.Crisis));
        }

        [Test]
        public void PhaseTransition_CrisisToCollapse_At270Seconds()
        {
            _timeline.StartTimeline();
            _timeline.Tick(270f);
            
            Assert.That(_timeline.CurrentPhase, Is.EqualTo(TimelinePhase.Collapse));
        }

        [Test]
        public void PhaseTransition_CollapseToChoiceLock_At300Seconds()
        {
            _timeline.StartTimeline();
            _timeline.Tick(300f);
            
            Assert.That(_timeline.CurrentPhase, Is.EqualTo(TimelinePhase.ChoiceLock));
        }

        // ── Event Emission Tests ───────────────────────────────────────────────

        [Test]
        public void PhaseTransition_EmitsTimelinePhaseChangedEvent_At90Seconds()
        {
            TimelinePhaseChangedEvent? received = null;
            var sub = _timeline.OnTimelinePhaseChanged.Subscribe(e => received = e);
            
            _timeline.StartTimeline();
            _timeline.Tick(90f);
            
            Assert.That(received, Is.Not.Null);
            Assert.That(received.Value.PreviousPhase, Is.EqualTo(TimelinePhase.Stability));
            Assert.That(received.Value.NewPhase, Is.EqualTo(TimelinePhase.Tension));
            Assert.That(received.Value.ElapsedTime, Is.EqualTo(90f));
            sub.Dispose();
        }

        [Test]
        public void PhaseTransition_EmitsTimelinePhaseChangedEvent_At180Seconds()
        {
            var allEvents = new System.Collections.Generic.List<TimelinePhaseChangedEvent>();
            var sub = _timeline.OnTimelinePhaseChanged.Subscribe(e => allEvents.Add(e));
            
            _timeline.StartTimeline();
            _timeline.Tick(90f);  // Stability → Tension
            _timeline.Tick(90f);  // Tension → Crisis (total 180s)
            
            // Last event should be Tension → Crisis
            Assert.That(allEvents.Count, Is.GreaterThan(0));
            var last = allEvents[allEvents.Count - 1];
            Assert.That(last.PreviousPhase, Is.EqualTo(TimelinePhase.Tension));
            Assert.That(last.NewPhase, Is.EqualTo(TimelinePhase.Crisis));
            sub.Dispose();
        }

        [Test]
        public void PhaseTransition_EmitsTimelinePhaseChangedEvent_At270Seconds()
        {
            var allEvents = new System.Collections.Generic.List<TimelinePhaseChangedEvent>();
            var sub = _timeline.OnTimelinePhaseChanged.Subscribe(e => allEvents.Add(e));
            
            _timeline.StartTimeline();
            _timeline.Tick(90f);  // Stability → Tension
            _timeline.Tick(90f);  // Tension → Crisis
            _timeline.Tick(90f);  // Crisis → Collapse (total 270s)
            
            Assert.That(allEvents.Count, Is.GreaterThan(0));
            var last = allEvents[allEvents.Count - 1];
            Assert.That(last.PreviousPhase, Is.EqualTo(TimelinePhase.Crisis));
            Assert.That(last.NewPhase, Is.EqualTo(TimelinePhase.Collapse));
            sub.Dispose();
        }

        [Test]
        public void PhaseTransition_EmitsTimelinePhaseChangedEvent_At300Seconds()
        {
            TimelinePhaseChangedEvent? received = null;
            var sub = _timeline.OnTimelinePhaseChanged.Subscribe(e => received = e);
            
            _timeline.StartTimeline();
            _timeline.Tick(90f);   // Stability → Tension
            _timeline.Tick(90f);   // Tension → Crisis
            _timeline.Tick(90f);   // Crisis → Collapse
            _timeline.Tick(30f);   // Collapse → ChoiceLock (total 300s)
            
            Assert.That(received, Is.Not.Null);
            Assert.That(received.Value.PreviousPhase, Is.EqualTo(TimelinePhase.Collapse));
            Assert.That(received.Value.NewPhase, Is.EqualTo(TimelinePhase.ChoiceLock));
            sub.Dispose();
        }

        [Test]
        public void TimelineIncomplete_ShouldNotEmitEvent()
        {
            TimelinePhaseChangedEvent? received = null;
            var sub = _timeline.OnTimelinePhaseChanged.Subscribe(e => received = e);
            
            _timeline.StartTimeline();
            _timeline.Tick(60f);
            
            Assert.That(received, Is.Null);
            sub.Dispose();
        }

        // ── Reset Tests ─────────────────────────────────────────────────────────

        [Test]
        public void Reset_ReturnsToStability()
        {
            _timeline.StartTimeline();
            _timeline.Tick(200f);
            _timeline.Reset();
            
            Assert.That(_timeline.CurrentPhase, Is.EqualTo(TimelinePhase.Stability));
            Assert.That(_timeline.ElapsedTime, Is.EqualTo(0f));
        }

        [Test]
        public void Tick_DoesNotAdvance_WhenNotRunning()
        {
            _timeline.Tick(100f);
            
            Assert.That(_timeline.ElapsedTime, Is.EqualTo(0f));
            Assert.That(_timeline.CurrentPhase, Is.EqualTo(TimelinePhase.Stability));
        }

        // ── Capacity Tests ─────────────────────────────────────────────────────

        [Test]
        public void PhaseDuration_Is300Seconds()
        {
            Assert.That(_timeline.PhaseDuration, Is.EqualTo(300f));
        }

        [Test]
        public void GetCurrentCapacity_StabilityPhase_Returns4()
        {
            _timeline.StartTimeline();
            Assert.That(_timeline.GetCurrentCapacity(), Is.EqualTo(4));
        }

        [Test]
        public void GetCurrentCapacity_TensionPhase_Returns3()
        {
            _timeline.StartTimeline();
            _timeline.Tick(90f);
            Assert.That(_timeline.GetCurrentCapacity(), Is.EqualTo(3));
        }

        [Test]
        public void GetCurrentCapacity_CrisisPhase_Returns3()
        {
            _timeline.StartTimeline();
            _timeline.Tick(180f);
            Assert.That(_timeline.GetCurrentCapacity(), Is.EqualTo(3));
        }

        [Test]
        public void GetCurrentCapacity_CollapsePhase_Returns1()
        {
            _timeline.StartTimeline();
            _timeline.Tick(270f);
            Assert.That(_timeline.GetCurrentCapacity(), Is.EqualTo(1));
        }
    }
}