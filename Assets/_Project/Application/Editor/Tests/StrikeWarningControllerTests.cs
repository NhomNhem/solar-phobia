// Assets/_Project/Application/Editor/Tests/StrikeWarningControllerTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;

namespace SolarPhobia.Application.Tests
{
    // ── Mock ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Test double for IMapSpawnDirector that records the last call to UpdatePlayerPosition.
    /// Used by property test P7 to verify position pass-through.
    /// </summary>
    internal class MockMapSpawnDirector : IMapSpawnDirector
    {
        // ── Recorded values ────────────────────────────────────────
        public Vector2 LastPosition { get; private set; }
        public Bounds  LastBounds   { get; private set; }
        public int     CallCount    { get; private set; }

        // ── IMapSpawnDirector (stubs) ──────────────────────────────
        public int Seed => 0;
        public Observable<bool>   OnStrikeWarning => Observable.Empty<bool>();
        public Observable<string> OnEnterCover    => Observable.Empty<string>();
        public Observable<string> OnExitCover     => Observable.Empty<string>();

        public void Initialize(int seed) { }

        public SolarPhobia.Application.Services.Map.ChunkData GenerateChunk(int index)
            => default;

        public void UpdatePlayerPosition(Vector2 position, Bounds bounds)
        {
            LastPosition = position;
            LastBounds   = bounds;
            CallCount++;
        }
    }

    // ── Unit Tests ────────────────────────────────────────────────────────────

    /// <summary>
    /// Unit and property-based tests for StrikeWarningController.
    /// Implements TR-player-009: Strike Warning Integration.
    /// </summary>
    [TestFixture]
    public class StrikeWarningControllerTests
    {
        private StrikeWarningController _controller;
        private MockMapSpawnDirector    _mockDirector;

        [SetUp]
        public void Setup()
        {
            _mockDirector = new MockMapSpawnDirector();
            _controller   = new StrikeWarningController(_mockDirector);
        }

        // ── 3.1 Unit Tests ────────────────────────────────────────

        /// <summary>
        /// Validates: Requirements 1.5
        /// Non-NightMovement mode must suppress all warnings — IsWarningActive stays false
        /// and ActiveWarnings stays empty when called with DayUI.
        /// </summary>
        [Test]
        public void PhaseGate_NonNightMovement_WarningStaysFalse()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, Vector2.zero);

            Assert.IsFalse(_controller.IsWarningActive.CurrentValue,
                "IsWarningActive must remain false in DayUI mode");
            Assert.AreEqual(0, _controller.ActiveWarnings.Count,
                "ActiveWarnings must remain empty in DayUI mode");
        }

        /// <summary>
        /// Validates: Requirements 1.5, 4.3
        /// A warning registered in NightMovement must be cleared when mode switches to DayUI.
        /// </summary>
        [Test]
        public void PhaseGate_NonNightMovement_ClearsExistingWarning()
        {
            // Register a warning in NightMovement
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, Vector2.zero);
            Assert.IsTrue(_controller.IsWarningActive.CurrentValue, "Precondition: warning must be active");

            // Switch to DayUI — must clear
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, Vector2.zero);

            Assert.IsFalse(_controller.IsWarningActive.CurrentValue,
                "Warning must clear when mode leaves NightMovement");
            Assert.AreEqual(0, _controller.ActiveWarnings.Count,
                "ActiveWarnings must be empty after phase exit");
        }

        /// <summary>
        /// Validates: Requirements 1.3, 1.4, 4.1, 4.4
        /// Sending one true then one false in NightMovement must leave the list empty
        /// and IsWarningActive false.
        /// </summary>
        [Test]
        public void RoundTrip_SingleTrueThenFalse_EmptyList()
        {
            _controller.OnStrikeWarningReceived(true,  PlayerInputMode.NightMovement, Vector2.zero);
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.AreEqual(0, _controller.ActiveWarnings.Count,
                "ActiveWarnings must be empty after round trip");
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue,
                "IsWarningActive must be false after round trip");
        }

        /// <summary>
        /// Validates: Requirements 4.4 (error handling — unbalanced false)
        /// Calling OnStrikeWarningReceived(false) on an empty controller must not throw
        /// and must leave count at 0.
        /// </summary>
        [Test]
        public void UnbalancedFalse_EmptyList_NoException()
        {
            Assert.DoesNotThrow(() =>
                _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, Vector2.zero),
                "Unbalanced false on empty list must not throw"
            );
            Assert.AreEqual(0, _controller.ActiveWarnings.Count,
                "Count must remain 0 after unbalanced false");
        }

        /// <summary>
        /// Validates: Requirements 4.3
        /// ClearAll() with active warnings must empty the list and set IsWarningActive to false.
        /// </summary>
        [Test]
        public void ClearAll_WithActiveWarnings_ClearsAll()
        {
            // Register 3 warnings
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Vector2(1f, 0f));
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Vector2(2f, 0f));
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Vector2(3f, 0f));
            Assert.AreEqual(3, _controller.ActiveWarnings.Count, "Precondition: 3 warnings registered");

            _controller.ClearAll();

            Assert.AreEqual(0, _controller.ActiveWarnings.Count,
                "ActiveWarnings must be empty after ClearAll");
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue,
                "IsWarningActive must be false after ClearAll");
        }

        /// <summary>
        /// Validates: Requirements 5.4
        /// Constructing StrikeWarningController with null MapDirector and calling
        /// ReportPlayerPosition in NightMovement must not throw.
        /// </summary>
        [Test]
        public void NullMapDirector_ReportPlayerPosition_DoesNotThrow()
        {
            var controllerWithNull = new StrikeWarningController(null);

            Assert.DoesNotThrow(() =>
                controllerWithNull.ReportPlayerPosition(
                    new Vector2(5f, 5f),
                    new Bounds(Vector3.zero, Vector3.one),
                    PlayerInputMode.NightMovement
                ),
                "ReportPlayerPosition with null MapDirector must not throw"
            );
        }

        // ── 3.2 Property Test — P1: Phase gate ───────────────────

        /// <summary>
        /// Validates: Requirements 1.5
        /// Feature: strike-warning-integration, Property 1: non-NightMovement suppresses warnings
        /// For 100 random (mode, warningValue) pairs where mode != NightMovement:
        ///   IsWarningActive must remain false, ActiveWarnings must remain empty.
        /// </summary>
        [Test]
        public void Property1_PhaseGate_NonNightMovement_SuppressesAllWarnings()
        {
            // Feature: strike-warning-integration, Property 1: non-NightMovement suppresses warnings
            var rng = new System.Random(42);
            var nonNightModes = new[] { PlayerInputMode.DayUI, PlayerInputMode.Disabled };

            for (int i = 0; i < 100; i++)
            {
                // Fresh controller each iteration
                var controller = new StrikeWarningController(_mockDirector);

                var mode         = nonNightModes[rng.Next(nonNightModes.Length)];
                var warningValue = rng.Next(2) == 1;
                var position     = new Vector2(
                    (float)(rng.NextDouble() * 200.0 - 100.0),
                    (float)(rng.NextDouble() * 200.0 - 100.0)
                );

                controller.OnStrikeWarningReceived(warningValue, mode, position);

                Assert.IsFalse(controller.IsWarningActive.CurrentValue,
                    $"[Iteration {i}] IsWarningActive must be false for mode={mode}, warningActive={warningValue}");
                Assert.AreEqual(0, controller.ActiveWarnings.Count,
                    $"[Iteration {i}] ActiveWarnings must be empty for mode={mode}, warningActive={warningValue}");
            }
        }

        // ── 3.3 Property Test — P2: Round trip ───────────────────

        /// <summary>
        /// Validates: Requirements 1.3, 1.4, 4.1, 4.4
        /// Feature: strike-warning-integration, Property 2: register then deregister is a round trip
        /// For 100 random N (1–10): send N true events then N false events in NightMovement.
        ///   ActiveWarnings.Count must be 0, IsWarningActive must be false.
        /// </summary>
        [Test]
        public void Property2_RoundTrip_NTrueThenNFalse_EmptyList()
        {
            // Feature: strike-warning-integration, Property 2: register then deregister is a round trip
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var controller = new StrikeWarningController(_mockDirector);
                int n = rng.Next(1, 11); // 1–10 inclusive

                // Send N true events
                for (int t = 0; t < n; t++)
                {
                    var pos = new Vector2(
                        (float)(rng.NextDouble() * 200.0 - 100.0),
                        (float)(rng.NextDouble() * 200.0 - 100.0)
                    );
                    controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, pos);
                }

                // Send N false events
                for (int f = 0; f < n; f++)
                {
                    controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, Vector2.zero);
                }

                Assert.AreEqual(0, controller.ActiveWarnings.Count,
                    $"[Iteration {i}, N={n}] ActiveWarnings must be 0 after N true + N false");
                Assert.IsFalse(controller.IsWarningActive.CurrentValue,
                    $"[Iteration {i}, N={n}] IsWarningActive must be false after round trip");
            }
        }

        // ── 3.4 Property Test — P3: Active list count ────────────

        /// <summary>
        /// Validates: Requirements 3.1, 3.4
        /// Feature: strike-warning-integration, Property 3: active list count tracks registrations
        /// For 100 random sequences of 5–15 true/false events in NightMovement:
        ///   ActiveWarnings.Count equals the running clamped count (each true +1, each false -1 clamped to 0).
        ///   Note: false events on an empty list are no-ops, so the final count may exceed max(0, trueCount - falseCount).
        /// </summary>
        [Test]
        public void Property3_ActiveListCount_TracksRegistrations()
        {
            // Feature: strike-warning-integration, Property 3: active list count tracks registrations
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var controller = new StrikeWarningController(_mockDirector);
                int length = rng.Next(5, 16); // 5–15 events

                // Track expected count using the same clamped-running-total logic as the controller:
                // each true increments, each false decrements but never below 0 (LIFO no-op on empty).
                int expectedCount = 0;

                for (int e = 0; e < length; e++)
                {
                    bool isTrue = rng.Next(2) == 1;
                    var pos = new Vector2(
                        (float)(rng.NextDouble() * 200.0 - 100.0),
                        (float)(rng.NextDouble() * 200.0 - 100.0)
                    );
                    controller.OnStrikeWarningReceived(isTrue, PlayerInputMode.NightMovement, pos);

                    if (isTrue)
                        expectedCount++;
                    else if (expectedCount > 0)
                        expectedCount--;
                }

                Assert.AreEqual(expectedCount, controller.ActiveWarnings.Count,
                    $"[Iteration {i}] Expected count={expectedCount} after {length} events");
            }
        }

        // ── 3.5 Property Test — P6: Phase exit clears all ────────

        /// <summary>
        /// Validates: Requirements 4.3
        /// Feature: strike-warning-integration, Property 6: phase exit clears all warnings
        /// For 100 random N (1–20) active warnings: call ClearAll().
        ///   ActiveWarnings.Count == 0, IsWarningActive == false.
        /// </summary>
        [Test]
        public void Property6_PhaseExit_ClearsAllWarnings()
        {
            // Feature: strike-warning-integration, Property 6: phase exit clears all warnings
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var controller = new StrikeWarningController(_mockDirector);
                int n = rng.Next(1, 21); // 1–20 warnings

                for (int w = 0; w < n; w++)
                {
                    var pos = new Vector2(
                        (float)(rng.NextDouble() * 200.0 - 100.0),
                        (float)(rng.NextDouble() * 200.0 - 100.0)
                    );
                    controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, pos);
                }

                controller.ClearAll();

                Assert.AreEqual(0, controller.ActiveWarnings.Count,
                    $"[Iteration {i}, N={n}] ActiveWarnings must be 0 after ClearAll");
                Assert.IsFalse(controller.IsWarningActive.CurrentValue,
                    $"[Iteration {i}, N={n}] IsWarningActive must be false after ClearAll");
            }
        }

        // ── 3.6 Property Test — P7: Position pass-through ────────

        /// <summary>
        /// Validates: Requirements 5.2
        /// Feature: strike-warning-integration, Property 7: position pass-through
        /// For 100 random (Vector2, Bounds) pairs in NightMovement mode:
        ///   UpdatePlayerPosition must be called with the exact values provided.
        /// </summary>
        [Test]
        public void Property7_PositionPassThrough_ExactValuesForwarded()
        {
            // Feature: strike-warning-integration, Property 7: position pass-through
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var mock       = new MockMapSpawnDirector();
                var controller = new StrikeWarningController(mock);

                var position = new Vector2(
                    (float)(rng.NextDouble() * 200.0 - 100.0),
                    (float)(rng.NextDouble() * 200.0 - 100.0)
                );
                var boundsSize = new Vector3(
                    (float)(rng.NextDouble() * (2.0 - 0.1) + 0.1),
                    (float)(rng.NextDouble() * (2.0 - 0.1) + 0.1),
                    0f
                );
                var bounds = new Bounds(Vector3.zero, boundsSize);

                controller.ReportPlayerPosition(position, bounds, PlayerInputMode.NightMovement);

                Assert.AreEqual(position, mock.LastPosition,
                    $"[Iteration {i}] Position must be forwarded exactly to UpdatePlayerPosition");
                Assert.AreEqual(bounds, mock.LastBounds,
                    $"[Iteration {i}] Bounds must be forwarded exactly to UpdatePlayerPosition");
                Assert.AreEqual(1, mock.CallCount,
                    $"[Iteration {i}] UpdatePlayerPosition must be called exactly once");
            }
        }
    }
}
