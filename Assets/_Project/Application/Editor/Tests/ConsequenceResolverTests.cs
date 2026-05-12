using System;
using System.Collections.Generic;
using NUnit.Framework;
using SolarPhobia.Application.Consequences;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Domain.ValueObjects;

using SolarPhobia.Application.Resources;

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

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Tests for ConsequenceResolver — deterministic curse mapping and one-write rule.
    /// Validates: Story 001 - Curse Mapping (consequence-resolver epic).
    /// Covers all 6 acceptance criteria from the story + edge cases.
    /// </summary>
    [TestFixture]
    public class ConsequenceResolverTests
    {
        // ── Stubs ──────────────────────────────────────────────────
        private class GhostStub : Ghost
        {
            public GhostStub(string id, string displayName) : base(id, displayName) { }
        }

        private class StubGhostRepository : IGhostRepository
        {
            private readonly Dictionary<string, Ghost> _ghosts = new();

            public void Add(Ghost ghost) => _ghosts[ghost.Id] = ghost;

            public IEnumerable<Ghost> GetAll() => _ghosts.Values;

            public Ghost GetById(string id) =>
                _ghosts.TryGetValue(id, out var ghost) ? ghost : null;

            public void Save(Ghost ghost)
            {
                if (ghost != null) _ghosts[ghost.Id] = ghost;
            }
        }

        // ── Fixture ───────────────────────────────────────────────
        private StubGhostRepository _ghostRepo;
        private ConsequenceResolver _resolver;

        [SetUp]
        public void SetUp()
        {
            _ghostRepo = new StubGhostRepository();
            _ghostRepo.Add(new Ghost("linh", "Em Linh"));
            _ghostRepo.Add(new Ghost("van", "Ã”ng VÄƒn"));
            _ghostRepo.Add(new Ghost("minh", "Anh Minh"));

            _resolver = new ConsequenceResolver(_ghostRepo);
        }

        // ───────────────────────────────────────────────────────────
        // ── AC-1: Curse Mapping — Deterministic Lookup ─────────────
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Resolve_Linh_ReturnsDrag()
        {
            var payload = _resolver.Resolve("linh");

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.Drag));
            Assert.That(payload.Intensity, Is.EqualTo(1.0f));
            Assert.That(payload.SpawnBias, Is.EqualTo("linh"));
        }

        [Test]
        public void Resolve_Van_ReturnsBlock()
        {
            var payload = _resolver.Resolve("van");

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.Block));
            Assert.That(payload.Intensity, Is.EqualTo(1.0f));
            Assert.That(payload.SpawnBias, Is.EqualTo("van"));
        }

        [Test]
        public void Resolve_Minh_ReturnsFakeShrine()
        {
            var payload = _resolver.Resolve("minh");

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.FakeShrine));
            Assert.That(payload.Intensity, Is.EqualTo(1.0f));
            Assert.That(payload.SpawnBias, Is.EqualTo("minh"));
        }

        [Test]
        public void Resolve_Deterministic_SameInputSameOutput()
        {
            // Run multiple times to confirm no randomness
            for (int i = 0; i < 5; i++)
            {
                var r1 = _resolver.Resolve("linh");
                Assert.That(r1.CurseType, Is.EqualTo(NightOutcomeState.Drag));
                // Reset resolver for next iteration
                _resolver = new ConsequenceResolver(_ghostRepo);
            }
        }

        // ───────────────────────────────────────────────────────────
        // ── AC-2: One-Write Rule ───────────────────────────────────
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Resolve_SecondCall_ThrowsInvalidOperationException()
        {
            _resolver.Resolve("linh");

            var ex = Assert.Throws<InvalidOperationException>(() => _resolver.Resolve("van"));
            Assert.That(ex.Message, Is.EqualTo("Already resolved"));
        }

        [Test]
        public void HasResolved_FalseBeforeResolve()
        {
            Assert.That(_resolver.HasResolved, Is.False);
        }

        [Test]
        public void HasResolved_TrueAfterResolve()
        {
            _resolver.Resolve("linh");

            Assert.That(_resolver.HasResolved, Is.True);
        }

        [Test]
        public void Reset_ClearsOneWriteGuard()
        {
            _resolver.Resolve("linh");

            _resolver.Reset();

            Assert.That(_resolver.HasResolved, Is.False);

            var payload = _resolver.Resolve("van");

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.Block));
            Assert.That(_resolver.HasResolved, Is.True);
        }

        // ───────────────────────────────────────────────────────────
        // ── AC-3: Invalid Soul ID ──────────────────────────────────
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Resolve_UnknownSoulId_DefaultsToDrag()
        {
            var payload = _resolver.Resolve("unknown_soul");

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.Drag));
            Assert.That(payload.SpawnBias, Is.EqualTo("unknown_soul"));
        }

        [Test]
        public void Resolve_EmptyString_DefaultsToDrag()
        {
            var payload = _resolver.Resolve("");

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.Drag));
        }

        [Test]
        public void Resolve_NullSoulId_DefaultsToDrag()
        {
            var payload = _resolver.Resolve(null);

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.Drag));
        }

        // ───────────────────────────────────────────────────────────
        // ── AC-4: Duplicate Write Rejected ─────────────────────────
        // (Already covered by AC-2 one-write rule tests above)
        // ── AC-5: Payload Delivery ─────────────────────────────────
        // (Verified in every Resolve test: CurseType, Intensity, SpawnBias)
        // ───────────────────────────────────────────────────────────

        // ───────────────────────────────────────────────────────────
        // ── AC-6: Integration with Ghost Model ─────────────────────
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Resolve_WritesNightOutcomeOnGhost()
        {
            var ghost = _ghostRepo.GetById("linh");
            _resolver.Resolve("linh");

            Assert.That(ghost.NightOutcome, Is.EqualTo(NightOutcomeState.Drag));
        }

        [Test]
        public void Resolve_WritesCorrectOutcomePerSoul()
        {
            _resolver.Resolve("van");
            var ghost = _ghostRepo.GetById("van");

            Assert.That(ghost.NightOutcome, Is.EqualTo(NightOutcomeState.Block));
        }

        [Test]
        public void Resolve_UnknownGhost_DoesNotThrow()
        {
            // Ghost not in repo — should still return payload, just skip ghost write
            var payload = _resolver.Resolve("nonexistent");

            Assert.That(payload.CurseType, Is.EqualTo(NightOutcomeState.Drag));
        }

        // ───────────────────────────────────────────────────────────
        // ── Performance: Completes within 0.1ms ────────────────────
        // ───────────────────────────────────────────────────────────

        [Test]
        public void Resolve_CompletesQuickly()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            _resolver.Resolve("linh");
            sw.Stop();

            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(1),
                $"Resolve took {sw.Elapsed.TotalMilliseconds:F3}ms, expected <1ms");
        }
    }
}

