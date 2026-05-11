using System;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Presentation.Player
{
    /// <summary>
    /// UI component that displays the strike warning icon near the reticle.
    /// Subscribes to IStrikeWarningController.IsWarningActive and controls
    /// the visibility and positioning of the warning icon UI element.
    /// Implements TR-player-011: Strike Warning Integration.
    /// </summary>
    public class StrikeWarningUI : MonoBehaviour, IInitializable, IDisposable
    {
        // ── Injected Dependencies ──────────────────────────────────
        [Inject] internal IStrikeWarningController _strikeWarningController;

        // ── UI Elements ────────────────────────────────────────────
        private VisualElement _rootElement;
        private VisualElement _warningIcon;
        private IDisposable _warningSubscription;

        // ── Constants ──────────────────────────────────────────────
        private const string WarningIconUssClass = "strike-warning-icon";

        // ── Unity Lifecycle ────────────────────────────────────────
        public void Initialize()
        {
            // Get reference to UI Document
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("[StrikeWarningUI] UIDocument component not found on GameObject.");
                return;
            }

            _rootElement = uiDocument.rootVisualElement;
            if (_rootElement == null)
            {
                Debug.LogError("[StrikeWarningUI] Root visual element not found.");
                return;
            }

            // Create warning icon element
            CreateWarningIcon();
            
            // Subscribe to warning active state
            SubscribeToWarningState();
        }

        public void Dispose()
        {
            _warningSubscription?.Dispose();
        }

        // ── Private Methods ────────────────────────────────────────
        private void CreateWarningIcon()
        {
            // Create a simple visual element for the warning icon
            _warningIcon = new VisualElement
            {
                name = "StrikeWarningIcon",
                style =
                {
                    position = Position.Absolute,
                    width = 32,
                    height = 32,
                    // Position slightly above center (reticle area)
                    top = Length.Percent(50),
                    left = Length.Percent(50),
                    // Offset by half the width and height to center the icon
                    marginTop = -16,
                    marginLeft = -16,
                    // Highest sort order to be above all other UI and screen effects
                }
            };

            // Add warning icon styling
            _warningIcon.AddToClassList(WarningIconUssClass);
            
            // Add to root
            _rootElement.Add(_warningIcon);
        }

        private void SubscribeToWarningState()
        {
            _warningSubscription = _strikeWarningController?.IsWarningActive
                .Subscribe(OnWarningActiveChanged);
        }

        private void OnWarningActiveChanged(bool isActive)
        {
            if (_warningIcon == null) return;
            
            // Show/hide the warning icon
            _warningIcon.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
            
            // Optional: Add pulsing animation when active
            if (isActive)
            {
                _warningIcon.EnableInClassList("pulse-animation", true);
            }
            else
            {
                _warningIcon.EnableInClassList("pulse-animation", false);
            }
        }
    }
}