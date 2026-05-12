// Assets/_Project/Application/Services/InteractHandler.cs
using R3;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Player.Interactions
{
    /// <summary>
    /// Dispatches E-key contextual interactions based on raycast hit tag.
    /// Implements TR-player-005: E-Key Contextual Interact â€” Relic Pickup + Shrine Trigger.
    ///
    /// Tag mapping (case-sensitive, matches Unity tag names):
    ///   "CursedMound" â†’ OnInteract("relic")
    ///   "EndShrine"   â†’ OnInteract("shrine")
    ///   anything else â†’ silently ignored
    ///
    /// Phase gate: only active when mode == NightMovement.
    /// Strike telegraph state does NOT block interaction â€” relic pickup during a
    /// strike applies Time Drain immediately without cancelling the strike (AC-5).
    /// </summary>
    public class InteractHandler : IInteractHandler
    {
        // â”€â”€ Tag Constants â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <summary>Unity tag for cursed mound / bone relic pickup point.</summary>
        public const string TagCursedMound = "CursedMound";

        /// <summary>Unity tag for end shrine / win condition trigger.</summary>
        public const string TagEndShrine = "EndShrine";

        /// <summary>Interaction payload emitted when player picks up a bone relic.</summary>
        public const string PayloadRelic = "relic";

        /// <summary>Interaction payload emitted when player reaches the end shrine.</summary>
        public const string PayloadShrine = "shrine";

        // â”€â”€ R3 Reactive State â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private readonly Subject<string> _onInteract = new();

        // â”€â”€ Public Interface â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <inheritdoc/>
        public Observable<string> OnInteract => _onInteract;

        // â”€â”€ Constructor â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Inject]
        public InteractHandler() { }

        // â”€â”€ IInteractHandler â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <inheritdoc/>
        public void TryInteract(string hitTag, PlayerInputMode mode)
        {
            // Phase gate â€” only active in NightMovement
            if (mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            // Tag dispatch â€” silently ignore unknown tags and null/empty
            if (hitTag == TagCursedMound)
            {
                _onInteract.OnNext(PayloadRelic);
            }
            else if (hitTag == TagEndShrine)
            {
                _onInteract.OnNext(PayloadShrine);
            }
            // All other tags (including FalseSafeMound, null, empty) â†’ no event
        }
    }
}

