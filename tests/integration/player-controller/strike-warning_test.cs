// tests/integration/player-controller/strike-warning_test.cs
using System;
using System.Reflection;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace SolarPhobia.Tests.Integration.PlayerController
{
    /// <summary>
    /// Integration tests for the Strike Warning UI component.
    /// Tests the interaction between StrikeWarningUI and IStrikeWarningController.
    /// </summary>
    public class StrikeWarningUITests
    {
        private GameObject _gameObject;
        private StrikeWarningUI _uiComponent;
        private FakeStrikeWarningController _fakeController;
        private UIDocument _uiDocument;

        [SetUp]
        public void SetUp()
        {
            // Create a GameObject to hold the UI component
            _gameObject = new GameObject("StrikeWarningUITest");

            // Add UIDocument component (required for UIElements)
            _uiDocument = _gameObject.AddComponent<UIDocument>();
            // Note: We don't have a real UXML file, but the StrikeWarningUI creates its own visual elements.
            // The UIDocument is just needed to get a rootVisualElement.

            // Add the StrikeWarningUI component
            _uiComponent = _gameObject.AddComponent<StrikeWarningUI>();

            // Create a fake controller
            _fakeController = new FakeStrikeWarningController();

            // Inject the fake controller into the UI component
            // We use reflection to set the private field because the component uses [Inject] internal field.
            // In a real test with VContainer, we would use the container, but for simplicity we set the field directly.
            var strikeWarningControllerField = typeof(StrikeWarningUI)
                .GetField("_strikeWarningController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            strikeWarningControllerField.SetValue(_uiComponent, _fakeController);

            // Initialize the component (this will set up the UI and subscribe)
            _uiComponent.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            _uiComponent.Dispose();
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void IsWarningActive_True_ShowsWarningIcon()
        {
            // Arrange
            // Act: Set the warning to active
            _fakeController.SetIsWarningActive(true);

            // Assert: The warning icon should be visible
            Assert.That(_uiComponent.GetWarningIconDisplayStyle(), Is.EqualTo(DisplayStyle.Flex),
                "Warning icon should be visible when IsWarningActive is true");
        }

        [Test]
        public void IsWarningActive_False_HidesWarningIcon()
        {
            // Arrange
            // Start with active to ensure we can test hiding
            _fakeController.SetIsWarningActive(true);
            Assert.That(_uiComponent.GetWarningIconDisplayStyle(), Is.EqualTo(DisplayStyle.Flex));

            // Act: Set the warning to inactive
            _fakeController.SetIsWarningActive(false);

            // Assert: The warning icon should be hidden
            Assert.That(_uiComponent.GetWarningIconDisplayStyle(), Is.EqualTo(DisplayStyle.None),
                "Warning icon should be hidden when IsWarningActive is false");
        }

        [Test]
        public void MultipleToggle_ShowsAndHidesCorrectly()
        {
            // Arrange
            _fakeController.SetIsWarningActive(false);
            Assert.That(_uiComponent.GetWarningIconDisplayStyle(), Is.EqualTo(DisplayStyle.None));

            // Act & Assert: Toggle on
            _fakeController.SetIsWarningActive(true);
            Assert.That(_uiComponent.GetWarningIconDisplayStyle(), Is.EqualTo(DisplayStyle.Flex));

            // Act & Assert: Toggle off
            _fakeController.SetIsWarningActive(false);
            Assert.That(_uiComponent.GetWarningIconDisplayStyle(), Is.EqualTo(DisplayStyle.None));

            // Act & Assert: Toggle on again
            _fakeController.SetIsWarningActive(true);
            Assert.That(_uiComponent.GetWarningIconDisplayStyle(), Is.EqualTo(DisplayStyle.Flex));
        }
    }

    /// <summary>
    /// Fake implementation of IStrikeWarningController for testing.
    /// </summary>
    public class FakeStrikeWarningController : IStrikeWarningController
    {
        private readonly ReactiveProperty<bool> _isWarningActive = new ReactiveProperty<bool>(false);
        public ReadOnlyReactiveProperty<bool> IsWarningActive => _isWarningActive;

        public IReadOnlyList<StrikeWarning> ActiveWarnings => Array.Empty<StrikeWarning>();

        public void OnStrikeWarningReceived(bool warningActive, PlayerInputMode mode, Vector2 playerPosition)
        {
            // Not used in these tests
        }

        public void ReportPlayerPosition(Vector2 position, Bounds bounds, PlayerInputMode mode)
        {
            // Not used in these tests
        }

        public void ClearAll()
        {
            // Not used in these tests
        }

        /// <summary>
        /// Sets the IsWarningActive value for testing.
        /// </summary>
        public void SetIsWarningActive(bool value)
        {
            _isWarningActive.Value = value;
        }
    }

    /// <summary>
    /// Extension methods to access private members of StrikeWarningUI for testing.
    /// </summary>
    public static class StrikeWarningUIExtensions
    {
        public static DisplayStyle GetWarningIconDisplayStyle(this StrikeWarningUI uiComponent)
        {
            // Use reflection to get the private _warningIcon field
            var warningIconField = typeof(StrikeWarningUI)
                .GetField("_warningIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var warningIcon = warningIconField.GetValue(uiComponent) as VisualElement;
            if (warningIcon == null)
                return DisplayStyle.None;

            return warningIcon.style.display.value;
        }
    }
}