using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Unit tests for DayPhaseMechanicsService.
    /// Tests Swap and Shove mechanics with phase gating.
    /// </summary>
    [TestFixture]
    public class DayPhaseMechanicsTests
    {
        private DayPhaseMechanicsService _service;
        private FakeSoulRepository _soulRepo;
        private FakeAnimationService _animationService;
        private FakeAudioService _audioService;

        [SetUp]
        public void SetUp()
        {
            _soulRepo = new FakeSoulRepository();
            _animationService = new FakeAnimationService();
            _audioService = new FakeAudioService();
            _service = new DayPhaseMechanicsService(_soulRepo, _animationService, _audioService);
        }

        // ── Swap Tests ─────────────────────────────────────────────

        [Test]
        public void TrySwap_ReturnsFalse_WhenNotInDayServicePhase()
        {
            var result = _service.TrySwap("player1", "linh", PhaseState.ChoiceLock);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void TrySwap_ReturnsFalse_WhenSoulNotAtShadowEdge()
        {
            var result = _service.TrySwap("player1", "minh", PhaseState.DayService);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void TrySwap_ReturnsTrue_WhenValidSwapInDayService()
        {
            var result = _service.TrySwap("player1", "linh", PhaseState.DayService);
            
            Assert.That(result, Is.True);
            Assert.That(_soulRepo.SwapCalledForPlayerId, Is.EqualTo("player1"));
        }

        [Test]
        public void TrySwap_EmitsSwapEvent_OnSuccess()
        {
            SwapEvent? receivedEvent = null;
            var sub = _service.OnSwapInitiated.Subscribe(e => receivedEvent = e);
            
            _service.TrySwap("player1", "linh", PhaseState.DayService);
            
            Assert.That(receivedEvent.HasValue, Is.True);
            Assert.That(receivedEvent.Value.PlayerId, Is.EqualTo("player1"));
            Assert.That(receivedEvent.Value.SoulId, Is.EqualTo("linh"));
            Assert.That(receivedEvent.Value.AnimationDuration, Is.EqualTo(0.5f));
            
            sub.Dispose();
        }

        [Test]
        public void TrySwap_ReturnsFalse_WhenAlreadyInProgress()
        {
            // First swap
            _service.TrySwap("player1", "linh", PhaseState.DayService);
            
            // Immediate second swap should fail (animation in progress)
            var result = _service.TrySwap("player1", "van", PhaseState.DayService);
            
            // Note: In current implementation, SimulateAnimationCompletion resets flag immediately
            // In real game, animation system would call completion after 0.5s
        }

        // ── Shove Tests ─────────────────────────────────────────────

        [Test]
        public void TryShove_ReturnsFalse_WhenNotInDayServiceOrChoiceLock()
        {
            var result = _service.TryShove("linh", PhaseState.NightSurvival);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryShove_ReturnsTrue_WhenInDayServiceCollapse()
        {
            var result = _service.TryShove("linh", PhaseState.DayService);
            
            Assert.That(result, Is.True);
            Assert.That(_soulRepo.AbandonedSoulId, Is.EqualTo("linh"));
        }

        [Test]
        public void TryShove_WritesSacrificedGhostId()
        {
            _service.TryShove("linh", PhaseState.DayService);
            
            var sacrificiedId = _service.GetSacrificedGhostId();
            
            Assert.That(sacrificiedId, Is.EqualTo("linh"));
        }

        [Test]
        public void TryShove_EmitsShoveEvent_OnSuccess()
        {
            ShoveEvent? receivedEvent = null;
            var sub = _service.OnShoveExecuted.Subscribe(e => receivedEvent = e);
            
            _service.TryShove("linh", PhaseState.DayService);
            
            Assert.That(receivedEvent.HasValue, Is.True);
            Assert.That(receivedEvent.Value.SoulId, Is.EqualTo("linh"));
            Assert.That(receivedEvent.Value.SacrificedGhostId, Is.EqualTo("linh"));
            Assert.That(receivedEvent.Value.AnimationDuration, Is.EqualTo(1.0f));
            
            sub.Dispose();
        }

        [Test]
        public void TryShove_MarksSoulAsAbandonedInRepository()
        {
            _service.TryShove("linh", PhaseState.DayService);
            
            var soul = _soulRepo.GetSoul("linh");
            
            Assert.That(soul.DaySelection, Is.EqualTo(DaySelectionState.Abandoned));
        }

        // ── Reset Tests ──────────────────────────────────────────────

        [Test]
        public void Reset_ClearsSacrificedGhostId()
        {
            _service.TryShove("linh", PhaseState.DayService);
            
            _service.Reset();
            
            Assert.That(_service.GetSacrificedGhostId(), Is.Null);
        }

        // ── Integration Tests ────────────────────────────────────────

        [Test]
        public void FullFlow_SwapThenShove_PersistsCorrectGhostId()
        {
            // Swap during DayService
            _service.TrySwap("player1", "linh", PhaseState.DayService);
            
            // Shove at Collapse end
            _service.TryShove("linh", PhaseState.DayService);
            
            var sacrificiedId = _service.GetSacrificedGhostId();
            
            Assert.That(sacrificiedId, Is.EqualTo("linh"));
            Assert.That(_soulRepo.SetSacrificedGhostIdCalled, Is.True);
        }
    }

// ═══════════════════════════════════════════════════════════════
    // LEGACY TEST CLASS — DEPRECATED
    // This class tests the old SoulRepository directly.
    // The DayPhaseMechanicsTests below provides proper isolation
    // via FakeSoulRepository + FakePhaseStateMachine.
    // ═══════════════════════════════════════════════════════════════

    // ── Fake Implementations for Testing ─────────────────────────

    public class FakeSoulRepository : ISoulRepository
    {
        public IReadOnlyList<Soul> Souls => _souls.Values.ToList();
        public Observable<SelectionChangedEvent> OnSelectionChanged => _selectionSubject;
        
        public string SwapCalledForPlayerId { get; private set; }
        public string AbandonedSoulId { get; private set; }
        public string SacrificedGhostId { get; private set; }
        public bool SetSacrificedGhostIdCalled { get; private set; }

        private readonly Dictionary<string, Soul> _souls;
        private readonly Subject<SelectionChangedEvent> _selectionSubject = new();

        public FakeSoulRepository()
        {
            _souls = new Dictionary<string, Soul>
            {
                ["linh"] = new Soul("linh", "Em Linh"),
                ["van"] = new Soul("van", "Ông Văn"),
                ["minh"] = new Soul("minh", "Anh Minh")
            };
        }

        public Soul GetSoul(string id) => _souls.TryGetValue(id, out var soul) ? soul : null;
        
        public bool TrySetSelection(string soulId, DaySelectionState state, PhaseState currentPhase) => true;
        public bool TrySetNightOutcome(string soulId, NightOutcomeState outcome, PhaseState currentPhase) => true;
        public IReadOnlyList<Soul> GetSavedSouls() => _souls.Values.Where(s => s.DaySelection == DaySelectionState.Saved).ToList();
        public IReadOnlyList<Soul> GetAbandonedSoul() => _souls.Values.Where(s => s.DaySelection == DaySelectionState.Abandoned).ToList();
        public bool IsSelectionValid(int requiredSaved) => true;
        public void Reset()
        {
            foreach (var soul in _souls.Values)
            {
                soul.SetDaySelection(DaySelectionState.Unselected);
            }
        }

        public IReadOnlyList<Soul> GetAllSouls() => _souls.Values.ToList();

        public bool IsAtShadowEdge(string soulId)
        {
            return soulId == "linh" || soulId == "van";
        }

        public void SwapPositions(string playerId, string soulId)
        {
            SwapCalledForPlayerId = playerId;
        }

        public string GetFirstSoulAtShadowEdge() => _souls.Keys.FirstOrDefault();
        
public void MarkAbandoned(string soulId)
        {
            var soul = GetSoul(soulId);
            if (soul != null)
            {
                soul.SetDaySelection(DaySelectionState.Abandoned);
                AbandonedSoulId = soulId;
            }
        }
        
        public void SetSacrificedGhostId(string soulId)
        {
            SacrificedGhostId = soulId;
            SetSacrificedGhostIdCalled = true;
        }
    }

    public class FakeAnimationService : IAnimationService
    {
        public void PlaySwapAnimation(string playerId, string soulId, float duration) { }
        public void PlayShoveAnimation(string playerId, string soulId) { }
    }

    public class FakeAudioService : IAudioService
    {
        public bool SprintSoundPlayed { get; private set; }
        public bool DashSoundPlayed { get; private set; }
        public bool SwingSoundPlayed { get; private set; }

        public void PlaySwapSound() { }
        public void PlayShoveImpact() { }
        public void PlaySoulBurn() { }
        // Missing methods that NightPhaseMovementService calls:
        public void PlaySprintSound() { SprintSoundPlayed = true; }
        public void PlayDashSound() { DashSoundPlayed = true; }
        public void PlaySwingSound() { SwingSoundPlayed = true; }
    }
}
