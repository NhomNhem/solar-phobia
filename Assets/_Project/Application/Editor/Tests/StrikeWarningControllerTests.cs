using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    internal class MockMapSpawnDirector : IMapSpawnDirector
    {
        public Float2 LastPosition { get; private set; }
        public Bounds2D LastBounds { get; private set; }
        public int CallCount { get; private set; }

        public int Seed => 0;
        public Observable<bool> OnStrikeWarning => Observable.Empty<bool>();
        public Observable<string> OnEnterCover => Observable.Empty<string>();
        public Observable<string> OnExitCover => Observable.Empty<string>();

        public void Initialize(int seed) { }
        public SolarPhobia.Application.Services.Map.ChunkData GenerateChunk(int index) => default;

        public void UpdatePlayerPosition(Float2 position, Bounds2D bounds)
        {
            LastPosition = position;
            LastBounds = bounds;
            CallCount++;
        }
    }

    [TestFixture]
    public class StrikeWarningControllerTests
    {
        private StrikeWarningController _controller;
        private MockMapSpawnDirector _mockDirector;

        [SetUp]
        public void Setup()
        {
            _mockDirector = new MockMapSpawnDirector();
            _controller = new StrikeWarningController(_mockDirector);
        }

        [Test]
        public void PhaseGate_NonNightMovement_WarningStaysFalse()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, new Float2(0f, 0f));
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
            Assert.AreEqual(0, _controller.ActiveWarnings.Count);
        }

        [Test]
        public void PhaseGate_NonNightMovement_ClearsExistingWarning()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.IsTrue(_controller.IsWarningActive.CurrentValue);
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, new Float2(0f, 0f));
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
            Assert.AreEqual(0, _controller.ActiveWarnings.Count);
        }

        [Test]
        public void RoundTrip_SingleTrueThenFalse_EmptyList()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.AreEqual(0, _controller.ActiveWarnings.Count);
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void UnbalancedFalse_EmptyList_NoException()
        {
            Assert.DoesNotThrow(() =>
                _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, new Float2(0f, 0f)));
            Assert.AreEqual(0, _controller.ActiveWarnings.Count);
        }

        [Test]
        public void ClearAll_WithActiveWarnings_ClearsAll()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(1f, 0f));
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(2f, 0f));
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(3f, 0f));
            Assert.AreEqual(3, _controller.ActiveWarnings.Count);

            _controller.ClearAll();

            Assert.AreEqual(0, _controller.ActiveWarnings.Count);
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void NullMapDirector_ReportPlayerPosition_DoesNotThrow()
        {
            var controllerWithNull = new StrikeWarningController(null);
            Assert.DoesNotThrow(() =>
                controllerWithNull.ReportPlayerPosition(
                    new Float2(5f, 5f),
                    new Bounds2D(new Float2(0f, 0f), new Float2(1f, 1f)),
                    PlayerInputMode.NightMovement));
        }

        [Test]
        public void Property1_PhaseGate_NonNightMovement_SuppressesAllWarnings()
        {
            var rng = new System.Random(42);
            var nonNightModes = new[] { PlayerInputMode.DayUI, PlayerInputMode.Disabled };

            for (int i = 0; i < 100; i++)
            {
                var controller = new StrikeWarningController(_mockDirector);
                var mode = nonNightModes[rng.Next(nonNightModes.Length)];
                var warningValue = rng.Next(2) == 1;
                var position = new Float2(
                    (float)(rng.NextDouble() * 200.0 - 100.0),
                    (float)(rng.NextDouble() * 200.0 - 100.0));

                controller.OnStrikeWarningReceived(warningValue, mode, position);

                Assert.IsFalse(controller.IsWarningActive.CurrentValue);
                Assert.AreEqual(0, controller.ActiveWarnings.Count);
            }
        }

        [Test]
        public void Property2_RoundTrip_NTrueThenNFalse_EmptyList()
        {
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var controller = new StrikeWarningController(_mockDirector);
                int n = rng.Next(1, 11);

                for (int t = 0; t < n; t++)
                {
                    var pos = new Float2(
                        (float)(rng.NextDouble() * 200.0 - 100.0),
                        (float)(rng.NextDouble() * 200.0 - 100.0));
                    controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, pos);
                }

                for (int f = 0; f < n; f++)
                {
                    controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, new Float2(0f, 0f));
                }

                Assert.AreEqual(0, controller.ActiveWarnings.Count);
                Assert.IsFalse(controller.IsWarningActive.CurrentValue);
            }
        }

        [Test]
        public void Property3_ActiveListCount_TracksRegistrations()
        {
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var controller = new StrikeWarningController(_mockDirector);
                int length = rng.Next(5, 16);
                int expectedCount = 0;

                for (int e = 0; e < length; e++)
                {
                    bool isTrue = rng.Next(2) == 1;
                    var pos = new Float2(
                        (float)(rng.NextDouble() * 200.0 - 100.0),
                        (float)(rng.NextDouble() * 200.0 - 100.0));
                    controller.OnStrikeWarningReceived(isTrue, PlayerInputMode.NightMovement, pos);

                    if (isTrue)
                    {
                        expectedCount++;
                    }
                    else if (expectedCount > 0)
                    {
                        expectedCount--;
                    }
                }

                Assert.AreEqual(expectedCount, controller.ActiveWarnings.Count);
            }
        }

        [Test]
        public void Property6_PhaseExit_ClearsAllWarnings()
        {
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var controller = new StrikeWarningController(_mockDirector);
                int n = rng.Next(1, 21);

                for (int w = 0; w < n; w++)
                {
                    var pos = new Float2(
                        (float)(rng.NextDouble() * 200.0 - 100.0),
                        (float)(rng.NextDouble() * 200.0 - 100.0));
                    controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, pos);
                }

                controller.ClearAll();

                Assert.AreEqual(0, controller.ActiveWarnings.Count);
                Assert.IsFalse(controller.IsWarningActive.CurrentValue);
            }
        }

        [Test]
        public void Property7_PositionPassThrough_ExactValuesForwarded()
        {
            var rng = new System.Random(42);

            for (int i = 0; i < 100; i++)
            {
                var mock = new MockMapSpawnDirector();
                var controller = new StrikeWarningController(mock);

                var position = new Float2(
                    (float)(rng.NextDouble() * 200.0 - 100.0),
                    (float)(rng.NextDouble() * 200.0 - 100.0));
                var bounds = new Bounds2D(
                    new Float2(0f, 0f),
                    new Float2(
                        (float)(rng.NextDouble() * (2.0 - 0.1) + 0.1),
                        (float)(rng.NextDouble() * (2.0 - 0.1) + 0.1)));

                controller.ReportPlayerPosition(position, bounds, PlayerInputMode.NightMovement);

                Assert.AreEqual(position.X, mock.LastPosition.X);
                Assert.AreEqual(position.Y, mock.LastPosition.Y);
                Assert.AreEqual(bounds.Center.X, mock.LastBounds.Center.X);
                Assert.AreEqual(bounds.Size.Y, mock.LastBounds.Size.Y);
                Assert.AreEqual(1, mock.CallCount);
            }
        }
    }
}
