using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Features.Resources;
using SolarPhobia.Domain.ValueObjects;


using SolarPhobia.Application.Features.Strike;

using SolarPhobia.Application.Features.Consequences.WaterTrap;

using SolarPhobia.Application.Features.Rituals;

using SolarPhobia.Application.Features.Shrines;

using SolarPhobia.Application.Features.Day;

using SolarPhobia.Application.Features.Phase.Flow;

using SolarPhobia.Application.Features.Player.State;

using SolarPhobia.Application.Features.Player.Input;

using SolarPhobia.Application.Features.Player.Interactions;

using SolarPhobia.Application.Features.Player.Cursor;

using SolarPhobia.Application.Features.Player.Events;

using SolarPhobia.Application.Features.Combat;

using SolarPhobia.Application.Features.Phase.Reset;

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class DayServiceSelectionLogicTests
    {
        private DaySelectionValidator _validator;

        private static readonly string Linh = "linh";
        private static readonly string Van = "van";
        private static readonly string Minh = "minh";

        private static readonly IReadOnlyList<string> DefaultPriority = new[] { Linh, Van, Minh };

        [SetUp]
        public void SetUp()
        {
            _validator = new DaySelectionValidator();
        }

        // ── Helpers ────────────────────────────────────────────────

        private static SoulSelectionState S(string soulId, DaySelectionState state) =>
            new(soulId, state);

        private static SoulSelectionState Saved(string soulId) => S(soulId, DaySelectionState.Saved);

        private static SoulSelectionState Abandoned(string soulId) => S(soulId, DaySelectionState.Abandoned);

        private static SoulSelectionState Unselected(string soulId) => S(soulId, DaySelectionState.Unselected);

        // ───────────────────────────────────────────────────────────
        // AC-1: Selection Validation (2 Saved, 1 Abandoned)
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Validate_2Saved1Abandoned_IsValid()
        {
            var selections = new List<SoulSelectionState>
            {
                Saved(Linh),
                Saved(Van),
                Abandoned(Minh)
            };

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.SavedCount, Is.EqualTo(2));
            Assert.That(result.AbandonedCount, Is.EqualTo(1));
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void Validate_2Saved1Abandoned_DifferentOrder_IsValid()
        {
            var selections = new List<SoulSelectionState>
            {
                Abandoned(Linh),
                Saved(Van),
                Saved(Minh)
            };

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.SavedCount, Is.EqualTo(2));
            Assert.That(result.AbandonedCount, Is.EqualTo(1));
        }

        // ───────────────────────────────────────────────────────────
        // AC-6: Invalid Pattern Block
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Validate_3Saved0Abandoned_IsInvalid()
        {
            var selections = new List<SoulSelectionState>
            {
                Saved(Linh),
                Saved(Van),
                Saved(Minh)
            };

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("2 Saved"));
        }

        [Test]
        public void Validate_0Saved3Abandoned_IsInvalid()
        {
            var selections = new List<SoulSelectionState>
            {
                Abandoned(Linh),
                Abandoned(Van),
                Abandoned(Minh)
            };

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("2 Saved"));
        }

        [Test]
        public void Validate_1Saved2Abandoned_IsInvalid()
        {
            var selections = new List<SoulSelectionState>
            {
                Saved(Linh),
                Abandoned(Van),
                Abandoned(Minh)
            };

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.SavedCount, Is.EqualTo(1));
            Assert.That(result.AbandonedCount, Is.EqualTo(2));
        }

        [Test]
        public void Validate_AllUnselected_IsInvalid()
        {
            var selections = new List<SoulSelectionState>
            {
                Unselected(Linh),
                Unselected(Van),
                Unselected(Minh)
            };

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_MixedUnselected_IsInvalid()
        {
            var selections = new List<SoulSelectionState>
            {
                Saved(Linh),
                Unselected(Van),
                Unselected(Minh)
            };

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.False);
        }

        // ───────────────────────────────────────────────────────────
        // AC-5: Auto-Complete
        // ───────────────────────────────────────────────────────────

        [Test]
        public void AutoComplete_FreshStart_LinhAndVanSaved_MinhAbandoned()
        {
            var current = new List<SoulSelectionState>
            {
                Unselected(Linh),
                Unselected(Van),
                Unselected(Minh)
            };

            var result = _validator.AutoComplete(current, DefaultPriority);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].State, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(result[0].SoulId, Is.EqualTo(Linh));
            Assert.That(result[1].State, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(result[1].SoulId, Is.EqualTo(Van));
            Assert.That(result[2].State, Is.EqualTo(DaySelectionState.Abandoned));
            Assert.That(result[2].SoulId, Is.EqualTo(Minh));
        }

        [Test]
        public void AutoComplete_PartialSelection_RespectsPriority()
        {
            var current = new List<SoulSelectionState>
            {
                Abandoned(Linh),
                Saved(Van),
                Unselected(Minh)
            };

            var result = _validator.AutoComplete(current, DefaultPriority);

            Assert.That(result[0].SoulId, Is.EqualTo(Linh));
            Assert.That(result[0].State, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(result[1].SoulId, Is.EqualTo(Van));
            Assert.That(result[1].State, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(result[2].SoulId, Is.EqualTo(Minh));
            Assert.That(result[2].State, Is.EqualTo(DaySelectionState.Abandoned));
        }

        [Test]
        public void AutoComplete_CustomPriority_RespectsOrder()
        {
            var customPriority = new[] { Minh, Linh, Van };
            var current = new List<SoulSelectionState>
            {
                Unselected(Linh),
                Unselected(Van),
                Unselected(Minh)
            };

            var result = _validator.AutoComplete(current, customPriority);

            Assert.That(result[0].SoulId, Is.EqualTo(Minh));
            Assert.That(result[0].State, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(result[1].SoulId, Is.EqualTo(Linh));
            Assert.That(result[1].State, Is.EqualTo(DaySelectionState.Saved));
            Assert.That(result[2].SoulId, Is.EqualTo(Van));
            Assert.That(result[2].State, Is.EqualTo(DaySelectionState.Abandoned));
        }

        [Test]
        public void AutoComplete_SingleSoul_ReturnsCorrectCount()
        {
            var current = new List<SoulSelectionState>
            {
                Unselected(Linh)
            };
            var priority = new[] { Linh };

            var result = _validator.AutoComplete(current, priority);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].State, Is.EqualTo(DaySelectionState.Saved));
        }

        // ───────────────────────────────────────────────────────────
        // State Tracking
        // ───────────────────────────────────────────────────────────

        [Test]
        public void SoulSelectionState_TracksSoulId()
        {
            var state = new SoulSelectionState(Linh, DaySelectionState.Saved);

            Assert.That(state.SoulId, Is.EqualTo(Linh));
        }

        [Test]
        public void SoulSelectionState_TracksSelectionState()
        {
            var state = new SoulSelectionState(Van, DaySelectionState.Abandoned);

            Assert.That(state.State, Is.EqualTo(DaySelectionState.Abandoned));
        }

        [Test]
        public void SoulSelectionState_DefaultIsUnselected()
        {
            var state = default(SoulSelectionState);

            Assert.That(state.State, Is.EqualTo(DaySelectionState.Unselected));
        }

        // ───────────────────────────────────────────────────────────
        // AC-8: Performance (within 0.05ms)
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Validate_CompletesWithinBudget()
        {
            var selections = new List<SoulSelectionState>
            {
                Saved(Linh),
                Saved(Van),
                Abandoned(Minh)
            };

            var sw = Stopwatch.StartNew();
            int iterations = 10000;
            for (int i = 0; i < iterations; i++)
            {
                _validator.Validate(selections);
            }
            sw.Stop();

            double avgMicroseconds = (sw.Elapsed.TotalMilliseconds * 1000.0) / iterations;
            Assert.That(avgMicroseconds, Is.LessThan(50.0),
                $"Average validate took {avgMicroseconds:F3}Î¼s, expected <50Î¼s (0.05ms)");
        }

        [Test]
        public void AutoComplete_CompletesWithinBudget()
        {
            var current = new List<SoulSelectionState>
            {
                Unselected(Linh),
                Unselected(Van),
                Unselected(Minh)
            };

            var sw = Stopwatch.StartNew();
            int iterations = 10000;
            for (int i = 0; i < iterations; i++)
            {
                _validator.AutoComplete(current, DefaultPriority);
            }
            sw.Stop();

            double avgMicroseconds = (sw.Elapsed.TotalMilliseconds * 1000.0) / iterations;
            Assert.That(avgMicroseconds, Is.LessThan(50.0),
                $"Average auto-complete took {avgMicroseconds:F3}Î¼s, expected <50Î¼s (0.05ms)");
        }

        // ───────────────────────────────────────────────────────────
        // Edge Cases
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Validate_EmptyList_IsInvalid()
        {
            var selections = new List<SoulSelectionState>();

            var result = _validator.Validate(selections);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.SavedCount, Is.EqualTo(0));
            Assert.That(result.AbandonedCount, Is.EqualTo(0));
        }

        [Test]
        public void Validate_NullList_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.Validate(null));
        }

        [Test]
        public void AutoComplete_NullPriority_ThrowsArgumentNullException()
        {
            var current = new List<SoulSelectionState>();

            Assert.Throws<ArgumentNullException>(() => _validator.AutoComplete(current, null));
        }

        [Test]
        public void AutoComplete_EmptyPriority_ReturnsEmpty()
        {
            var current = new List<SoulSelectionState>
            {
                Unselected(Linh)
            };
            var emptyPriority = Array.Empty<string>();

            var result = _validator.AutoComplete(current, emptyPriority);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_CanToggleBetweenStates()
        {
            var saved = new List<SoulSelectionState>
            {
                Saved(Linh),
                Saved(Van),
                Saved(Minh)
            };
            Assert.That(_validator.Validate(saved).IsValid, Is.False);

            var fixed_selection = new List<SoulSelectionState>
            {
                Saved(Linh),
                Saved(Van),
                Abandoned(Minh)
            };
            Assert.That(_validator.Validate(fixed_selection).IsValid, Is.True);
        }
    }
}






