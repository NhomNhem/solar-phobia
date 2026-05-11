// Assets/_Project/Presentation/Player/StrikeWarningView.cs
using System;
using R3;
using SolarPhobia.Application.Services;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace SolarPhobia.Presentation.Player
{
    /// <summary>
    /// Presentation layer: binds <see cref="IStrikeWarningController.IsWarningActive"/> to the
    /// Warning_Icon <see cref="VisualElement"/> inside the attached <see cref="UIDocument"/>.
    ///
    /// Canvas configuration (set in Inspector, never changed at runtime):
    ///   - Render Mode: Screen Space — Overlay
    ///   - PanelSettings Sort Order: 100 (above all Tier 4 post-processing, sort order ≤ 10)
    ///
    /// Implements Requirements 2.1, 2.2, 2.3.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class StrikeWarningView : MonoBehaviour
    {
        // ── Inspector ──────────────────────────────────────────────
        [SerializeField] private string _warningIconName = "warning-icon";

        // ── State ─────────────────────────────────────────────────
        private VisualElement _warningIcon;
        private IDisposable   _subscription;

        // ── Dependencies (injected via VContainer) ─────────────────
        [Inject] internal IStrikeWarningController _controller;

        // ── Unity Lifecycle ────────────────────────────────────────
        private void Start()
        {
            var document = GetComponent<UIDocument>();
            _warningIcon = document.rootVisualElement.Q<VisualElement>(_warningIconName);

            if (_warningIcon == null)
            {
                Debug.LogError($"[StrikeWarningView] VisualElement '{_warningIconName}' not found — disabling.");
                enabled = false;
                return;
            }

            _subscription = _controller.IsWarningActive
                .Subscribe(active =>
                    _warningIcon.style.display = active
                        ? DisplayStyle.Flex
                        : DisplayStyle.None);

            // Ensure icon starts hidden
            _warningIcon.style.display = DisplayStyle.None;
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}
