// Assets/_Project/Presentation/Player/PlayerController.cs
using System;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace SolarPhobia.Presentation.Player
{
    /// <summary>
    /// Player Controller handling strike warnings and relic pickups.
    /// Subscribes to <see cref="IPlayerInputHandler.CurrentMode"/> to detect NightMovement entry/exit,
    /// forwards <see cref="IMapSpawnDirector.OnStrikeWarning"/> events to
    /// <see cref="IStrikeWarningController"/>, fires relic and shrine interaction events via 
    /// <see cref="OnInteract"/>, and reports the player's world position
    /// each frame while in NightMovement mode.
    ///
    /// Movement, input handling, and all other player logic belong to separate stories
    /// in the player-controller epic and must NOT be added here.
    ///
    /// Implements TR-player-009: Strike Warning Integration and TR-player-012: Relic Pickup Integration.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Fired when the player interacts with an object (e.g., relic, shrine).
        /// Payload is the interaction type: "relic" or "shrine".
        /// </summary>
        public event Action<string> OnInteract;

        // ── Injected Dependencies ──────────────────────────────────
        [Inject] internal IMapSpawnDirector        _mapDirector;
        [Inject] internal IStrikeWarningController _strikeWarningController;
        [Inject] internal IPlayerInputHandler      _inputHandler;
        [Inject] internal IResourceEffectsService  _resourceEffectsService;

        // ── Private State ──────────────────────────────────────────
        private PlayerInputMode _mode;
        private IDisposable     _modeSubscription;
        private IDisposable     _strikeWarningSubscription;
        private Collider2D      _cachedCollider;

        // ── Unity Lifecycle ────────────────────────────────────────
        private void Start()
        {
            _cachedCollider = GetComponent<Collider2D>();

            // Cache initial mode
            _mode = _inputHandler.CurrentMode.CurrentValue;

            // Subscribe to future mode changes
            _modeSubscription = _inputHandler.CurrentMode
                .Subscribe(OnModeChanged);

            // If already in NightMovement on Start, subscribe to strike warnings immediately
            if (_mode == PlayerInputMode.NightMovement)
            {
                SubscribeToStrikeWarning();
            }
        }

        private void Update()
        {
            if (_mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            _strikeWarningController.ReportPlayerPosition(
                (Vector2)transform.position,
                _cachedCollider != null ? _cachedCollider.bounds : new Bounds(transform.position, Vector3.zero),
                _mode);
        }

        private void OnDestroy()
        {
            _modeSubscription?.Dispose();
            _modeSubscription = null;

            _strikeWarningSubscription?.Dispose();
            _strikeWarningSubscription = null;

            _strikeWarningController?.ClearAll();
        }

        // ── Private Methods ────────────────────────────────────────
        /// <summary>
        /// Handles mode transitions. Subscribes to strike warnings on NightMovement entry
        /// and disposes the subscription on exit.
        /// </summary>
        private void OnModeChanged(PlayerInputMode mode)
        {
            _mode = mode;

            if (mode == PlayerInputMode.NightMovement)
            {
                SubscribeToStrikeWarning();
            }
            else
            {
                _strikeWarningSubscription?.Dispose();
                _strikeWarningSubscription = null;
                _strikeWarningController.ClearAll();
            }
        }

        /// <summary>
        /// Creates the subscription to <see cref="IMapSpawnDirector.OnStrikeWarning"/>
        /// and forwards each event to <see cref="IStrikeWarningController.OnStrikeWarningReceived"/>.
        /// </summary>
        private void SubscribeToStrikeWarning()
        {
            _strikeWarningSubscription?.Dispose();
            _strikeWarningSubscription = _mapDirector.OnStrikeWarning
                .Subscribe(active =>
                    _strikeWarningController.OnStrikeWarningReceived(
                        active,
                        _mode,
                        (Vector2)transform.position));
        }

        /// <summary>
        /// Handles E-key input for contextual interactions (relic pickup, shrine trigger).
        /// Called by PlayerInputHandler on Interact action.
        /// </summary>
        private void OnInteractInput(InputAction.CallbackContext ctx)
        {
            if (_mode != PlayerInputMode.NightMovement) return;

            // Check what's in front of player
            var hit = Physics2D.Raycast(transform.position, transform.right, 2f);

            if (hit.collider.CompareTag("CursedMound"))
            {
                OnInteract?.Invoke("relic");
                _resourceEffectsService.HandleResourcePickup(ResourceType.NgocCot);
            }
            else if (hit.collider.CompareTag("EndShrine"))
            {
                OnInteract?.Invoke("shrine");
            }
        }
    }
}