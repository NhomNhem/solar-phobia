using System;
using System.Collections.Generic;
using NUnit.Framework;
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
    public class RitualAssignmentTests
    {
        // ───────────────────────────────────────────────────────────
        // AC-2: Ritual Assignment
        // ───────────────────────────────────────────────────────────

        [Test]
        public void AssignRitual_ValidSoul_Succeeds()
        {
            var service = new RitualAssignmentService();

            bool result = service.TryAssignRitual("linh", RitualType.Tea);

            Assert.That(result, Is.True);
            Assert.That(service.Assignments, Contains.Key("linh"));
            Assert.That(service.Assignments["linh"], Is.EqualTo(RitualType.Tea));
        }

        [Test]
        public void AssignRitual_InvalidSoulId_Fails()
        {
            var service = new RitualAssignmentService();

            bool result = service.TryAssignRitual("nonexistent", RitualType.Tea);

            Assert.That(result, Is.False);
            Assert.That(service.Assignments, Is.Empty);
        }

        [Test]
        public void AssignRitual_NullSoulId_Fails()
        {
            var service = new RitualAssignmentService();

            bool result = service.TryAssignRitual(null, RitualType.Tea);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AssignRitual_EmptySoulId_Fails()
        {
            var service = new RitualAssignmentService();

            bool result = service.TryAssignRitual("", RitualType.Tea);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AssignRitual_ReassignChangesRitualType()
        {
            var service = new RitualAssignmentService();
            service.TryAssignRitual("linh", RitualType.Tea);

            service.TryAssignRitual("linh", RitualType.Offering);

            Assert.That(service.Assignments["linh"], Is.EqualTo(RitualType.Offering));
        }

        // ───────────────────────────────────────────────────────────
        // Ritual Removal
        // ───────────────────────────────────────────────────────────

        [Test]
        public void RemoveRitual_Existing_Succeeds()
        {
            var service = new RitualAssignmentService();
            service.TryAssignRitual("linh", RitualType.Tea);

            bool removed = service.TryRemoveRitual("linh");

            Assert.That(removed, Is.True);
            Assert.That(service.Assignments, Does.Not.ContainKey("linh"));
        }

        [Test]
        public void RemoveRitual_NoAssignment_Fails()
        {
            var service = new RitualAssignmentService();

            bool removed = service.TryRemoveRitual("linh");

            Assert.That(removed, Is.False);
        }

        [Test]
        public void RemoveRitual_InvalidSoulId_Fails()
        {
            var service = new RitualAssignmentService();

            bool removed = service.TryRemoveRitual("nonexistent");

            Assert.That(removed, Is.False);
        }

        // ───────────────────────────────────────────────────────────
        // Clear
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Clear_RemovesAllAssignments()
        {
            var service = new RitualAssignmentService();
            service.TryAssignRitual("linh", RitualType.Tea);
            service.TryAssignRitual("van", RitualType.Incense);

            service.Clear();

            Assert.That(service.Assignments, Is.Empty);
        }

        // ───────────────────────────────────────────────────────────
        // Preferred Ritual Bonus
        // ───────────────────────────────────────────────────────────

        [Test]
        public void IsPreferredRitual_LinhTea_ReturnsTrue()
        {
            var service = new RitualAssignmentService();

            bool preferred = service.IsPreferredRitual("linh", RitualType.Tea);

            Assert.That(preferred, Is.True);
        }

        [Test]
        public void IsPreferredRitual_VanIncense_ReturnsTrue()
        {
            var service = new RitualAssignmentService();

            bool preferred = service.IsPreferredRitual("van", RitualType.Incense);

            Assert.That(preferred, Is.True);
        }

        [Test]
        public void IsPreferredRitual_MinhOffering_ReturnsTrue()
        {
            var service = new RitualAssignmentService();

            bool preferred = service.IsPreferredRitual("minh", RitualType.Offering);

            Assert.That(preferred, Is.True);
        }

        [Test]
        public void IsPreferredRitual_WrongRitual_ReturnsFalse()
        {
            var service = new RitualAssignmentService();

            bool preferred = service.IsPreferredRitual("linh", RitualType.Incense);

            Assert.That(preferred, Is.False);
        }

        [Test]
        public void IsPreferredRitual_UnknownSoul_ReturnsFalse()
        {
            var service = new RitualAssignmentService();

            bool preferred = service.IsPreferredRitual("unknown", RitualType.Tea);

            Assert.That(preferred, Is.False);
        }

        // ───────────────────────────────────────────────────────────
        // Graceful Degradation (no IResourceEffectApplier)
        // ───────────────────────────────────────────────────────────

        [Test]
        public void GracefulDegradation_NoEffectApplier_AssignsWithoutCrash()
        {
            var service = new RitualAssignmentService();

            Assert.DoesNotThrow(() =>
            {
                bool result = service.TryAssignRitual("linh", RitualType.Tea);
                Assert.That(result, Is.True);
            });
        }

        [Test]
        public void GracefulDegradation_AllowsAllRitualTypes()
        {
            var service = new RitualAssignmentService();

            Assert.That(service.TryAssignRitual("linh", RitualType.Tea), Is.True);
            Assert.That(service.TryAssignRitual("van", RitualType.Incense), Is.True);
            Assert.That(service.TryAssignRitual("minh", RitualType.Offering), Is.True);

            Assert.That(service.Assignments.Count, Is.EqualTo(3));
        }

        // ───────────────────────────────────────────────────────────
        // Edge Cases
        // ───────────────────────────────────────────────────────────

        [Test]
        public void MultipleSouls_SameRitualType_Allowed()
        {
            var service = new RitualAssignmentService();

            Assert.That(service.TryAssignRitual("linh", RitualType.Tea), Is.True);
            Assert.That(service.TryAssignRitual("van", RitualType.Tea), Is.True);

            Assert.That(service.Assignments.Count, Is.EqualTo(2));
        }

        [Test]
        public void AssignThenClear_ThenReassign_Works()
        {
            var service = new RitualAssignmentService();
            service.TryAssignRitual("linh", RitualType.Tea);
            service.Clear();

            bool result = service.TryAssignRitual("linh", RitualType.Offering);

            Assert.That(result, Is.True);
            Assert.That(service.Assignments["linh"], Is.EqualTo(RitualType.Offering));
        }

        [Test]
        public void Dispose_ClearsAssignments()
        {
            var service = new RitualAssignmentService();
            service.TryAssignRitual("linh", RitualType.Tea);

            service.Dispose();

            Assert.That(service.Assignments, Is.Empty);
        }
    }
}


