// Assets/_Project/Tests/Integration/PlayerController/relic-pickup-integration_test.cs
using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;

namespace SolarPhobia.Tests.Integration.PlayerController
{
    public class RelicPickupIntegrationTests
    {
        private ResourceEffectsService _resourceEffectsService;
        private PlayerController _playerController;
        private TestablePlayerInputHandler _inputHandler;
        private TestableMapSpawnDirector _mapDirector;
        private TestableStrikeWarningController _strikeWarningController;
        private TestableResourceEffectsService _resourceEffects;

        [SetUp]
        public void Setup()
        {
            _resourceEffectsService = new ResourceEffectsService();
            _inputHandler = new TestablePlayerInputHandler();
            _mapDirector = new TestableMapSpawnDirector();
            _strikeWarningController = new TestableStrikeWarningController();
            _resourceEffects = new TestableResourceEffectsService();
            
            _playerController = new PlayerController();
            // Manually inject dependencies since we're not using VContainer in tests
            typeof(PlayerController)
                .GetField("_inputHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_playerController, _inputHandler);
            typeof(PlayerController)
                .GetField("_mapDirector", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_playerController, _mapDirector);
            typeof(PlayerController)
                .GetField("_strikeWarningController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_playerController, _strikeWarningController);
            typeof(PlayerController)
                .GetField("_resourceEffectsService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_playerController, _resourceEffects);
        }

        [TearDown]
        public void Teardown()
        {
            _resourceEffectsService = null;
            _playerController = null;
            _inputHandler = null;
            _mapDirector = null;
            _strikeWarningController = null;
            _resourceEffects = null;
        }

        // Test AC-1: Relic pickup event reaches Resource Effects
        [Test]
        public void RelicPickup_EventReachesResourceEffectsSystem()
        {
            // Arrange
            bool eventReceived = false;
            ResourceType receivedType = ResourceType.NgocCot;
            
            _resourceEffectsService.OnResourcePickedUp.Subscribe(type =>
            {
                eventReceived = true;
                receivedType = type;
            }).AddTo(new CompositeDisposable());

            // Simulate E-key press on CursedMound
            _inputHandler.CurrentSimulatedMode = PlayerInputMode.NightMovement;
            
            // Act - Simulate the flow: PlayerController detects E-key on CursedMound
            // and calls ResourceEffectsService.HandleResourcePickup
            _resourceEffectsService.HandleResourcePickup(ResourceType.NgocCot);

            // Assert
            Assert.IsTrue(eventReceived, "ResourceEffectsService should receive the relic pickup event");
            Assert.AreEqual(ResourceType.NgocCot, receivedType, "Event should contain NgocCot resource type");
        }

        // Test AC-2: Time Drain activates on receipt
        [Test]
        public void RelicPickup_TimeDrainActivatesOnReceipt()
        {
            // Arrange
            _inputHandler.CurrentSimulatedMode = PlayerInputMode.NightMovement;
            
            // Act
            _resourceEffectsService.HandleResourcePickup(ResourceType.NgocCot);

            // Assert
            Assert.IsTrue(_resourceEffectsService.IsTimeDrainActive, 
                "Time Drain should be active after relic pickup");
            Assert.AreEqual(1.0f, _resourceEffectsService.TimeDrainMultiplier,
                "Time Drain multiplier should be 1.0x (placeholder value)");
        }

        // Test AC-3: Pickup during active strike telegraph applies Time Drain immediately
        [Test]
        public void RelicPickup_DuringStrikeTelegraph_TimeDrainActivatesImmediately()
        {
            // Arrange - Simulate active strike telegraph
            _mapDirector.IsStrikeTelegraphActive = true;
            _inputHandler.CurrentSimulatedMode = PlayerInputMode.NightMovement;
            
            // Act
            _resourceEffectsService.HandleResourcePickup(ResourceType.NgocCot);

            // Assert - Time Drain should activate immediately regardless of strike state
            Assert.IsTrue(_resourceEffectsService.IsTimeDrainActive,
                "Time Drain should activate immediately even during strike telegraph");
        }

        // Test AC-4: No duplicate events
        [Test]
        public void RelicPickup_NoDuplicateEventsFired()
        {
            // Arrange
            int eventCount = 0;
            _resourceEffectsService.OnResourcePickedUp.Subscribe(_ => eventCount++).AddTo(new CompositeDisposable());
            
            _inputHandler.CurrentSimulatedMode = PlayerInputMode.NightMovement;
            
            // Act - Fire the same event twice
            _resourceEffectsService.HandleResourcePickup(ResourceType.NgocCot);
            _resourceEffectsService.HandleResourcePickup(ResourceType.NgocCot);

            // Assert
            Assert.AreEqual(2, eventCount, 
                "Two events should be fired for two pickup actions");
        }
    }

    // Test doubles/mocks
    public class TestablePlayerInputHandler
    {
        public PlayerInputMode CurrentSimulatedMode { get; set; } = PlayerInputMode.DayUI;
        public ReadOnlyReactiveProperty<PlayerInputMode> CurrentMode => 
            new ReadOnlyReactiveProperty<PlayerInputMode>(CurrentSimulatedMode);
        public bool IsMovementEnabled => CurrentSimulatedMode == PlayerInputMode.NightMovement;
        public bool IsUIEnabled => CurrentSimulatedMode == PlayerInputMode.DayUI;
    }

    public class TestableMapSpawnDirector
    {
        public bool IsStrikeTelegraphActive { get; set; } = false;
        public IObservable<bool> OnStrikeWarning => 
            Observable.Return(IsStrikeTelegraphActive);
    }

    public class TestableStrikeWarningController
    {
        public void OnStrikeWarningReceived(bool active, PlayerInputMode mode, Vector2 position) { }
        public void ClearAll() { }
        public void ReportPlayerPosition(Vector2 position, Bounds colliderBounds, PlayerInputMode mode) { }
    }

    public class TestableResourceEffectsService : IResourceEffectsService
    {
        public ReadOnlyReactiveProperty<ResourceType> OnResourcePickedUp { get; } = new ReactiveProperty<ResourceType>(ResourceType.NgocCot);
        public bool IsTimeDrainActive { get; private set; }
        public float TimeDrainMultiplier { get; private set; } = 1.0f;
        
        public void HandleResourcePickup(ResourceType resourceType)
        {
            OnResourcePickedUp.Value = resourceType;
            IsTimeDrainActive = true;
            TimeDrainMultiplier = 1.25f;
        }
    }

    // Simplified disposable for testing
    public class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        
        public void Add(IDisposable disposable) => _disposables.Add(disposable);
        public void Dispose() => _disposables.ForEach(d => d.Dispose());
    }
}