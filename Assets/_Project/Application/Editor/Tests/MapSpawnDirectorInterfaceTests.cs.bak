// Assets/_Project/Application/Editor/Tests/MapSpawnDirectorInterfaceTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using UnityEngine;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-007 — IMapSpawnDirector Player Controller signals.
    /// Story 006: IMapSpawnDirector Interface — cover/strike overlap events.
    /// </summary>
    [TestFixture]
    public class MapSpawnDirectorInterfaceTests
    {
        private MapSpawnDirector _director;
        private List<bool>   _strikeWarnings;
        private List<string> _coverEnterEvents;
        private List<string> _coverExitEvents;

        [SetUp]
        public void Setup()
        {
            _director         = new MapSpawnDirector();
            _strikeWarnings   = new List<bool>();
            _coverEnterEvents = new List<string>();
            _coverExitEvents  = new List<string>();

            _director.OnStrikeWarning.Subscribe(v => _strikeWarnings.Add(v));
            _director.OnEnterCover.Subscribe(t   => _coverEnterEvents.Add(t));
            _director.OnExitCover.Subscribe(t    => _coverExitEvents.Add(t));
        }

        // ── OnStrikeWarning ────────────────────────────────────────

        [Test]
        public void OnStrikeWarning_FiresTrue_WhenNotified()
        {
            _director.NotifyStrikeWarning(true);

            Assert.AreEqual(1, _strikeWarnings.Count);
            Assert.IsTrue(_strikeWarnings[0]);
        }

        [Test]
        public void OnStrikeWarning_FiresFalse_WhenCancelled()
        {
            _director.NotifyStrikeWarning(true);
            _director.NotifyStrikeWarning(false);

            Assert.AreEqual(2, _strikeWarnings.Count);
            Assert.IsFalse(_strikeWarnings[1]);
        }

        // ── OnEnterCover ───────────────────────────────────────────

        [Test]
        public void OnEnterCover_FiresWithTag_WhenNotified()
        {
            _director.NotifyCoverEnter("MoThuong");

            Assert.AreEqual(1, _coverEnterEvents.Count);
            Assert.AreEqual("MoThuong", _coverEnterEvents[0]);
        }

        [Test]
        public void OnEnterCover_FiresWithFalseSafeMoundTag()
        {
            _director.NotifyCoverEnter("FalseSafeMound");

            Assert.AreEqual("FalseSafeMound", _coverEnterEvents[0]);
        }

        // ── OnExitCover ────────────────────────────────────────────

        [Test]
        public void OnExitCover_FiresWithTag_WhenNotified()
        {
            _director.NotifyCoverExit("MoThuong");

            Assert.AreEqual(1, _coverExitEvents.Count);
            Assert.AreEqual("MoThuong", _coverExitEvents[0]);
        }

        // ── UpdatePlayerPosition ───────────────────────────────────

        [Test]
        public void UpdatePlayerPosition_DoesNotThrow()
        {
            _director.Initialize(42);

            Assert.DoesNotThrow(() =>
                _director.UpdatePlayerPosition(
                    new Vector2(5f, 0f),
                    new Bounds(Vector3.zero, Vector3.one)
                )
            );
        }

        // ── Interface completeness ─────────────────────────────────

        [Test]
        public void IMapSpawnDirector_HasOnStrikeWarning()
        {
            IMapSpawnDirector iface = _director;
            Assert.IsNotNull(iface.OnStrikeWarning);
        }

        [Test]
        public void IMapSpawnDirector_HasOnEnterCover()
        {
            IMapSpawnDirector iface = _director;
            Assert.IsNotNull(iface.OnEnterCover);
        }

        [Test]
        public void IMapSpawnDirector_HasOnExitCover()
        {
            IMapSpawnDirector iface = _director;
            Assert.IsNotNull(iface.OnExitCover);
        }

        [Test]
        public void IMapSpawnDirector_HasUpdatePlayerPosition()
        {
            // Verify method exists and is callable via interface
            IMapSpawnDirector iface = _director;
            _director.Initialize(1);
            Assert.DoesNotThrow(() =>
                iface.UpdatePlayerPosition(Vector2.zero, new Bounds()));
        }
    }
}
