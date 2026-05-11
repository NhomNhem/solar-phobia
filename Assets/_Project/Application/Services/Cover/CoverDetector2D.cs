using R3;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Services.Cover
{
    public class CoverDetector2D : ICoverDetector2D
    {
        // ── Tag Constants ──────────────────────────────────────────
        /// <summary>Unity tag for safe Mộ Gió cover.</summary>
        public const string TagMoThuong = "MoThuong";

        /// <summary>Unity tag for FalseSafeMound (cover + warning).</summary>
        public const string TagFalseSafeMound = "FalseSafeMound";

        // ── R3 Reactive State ──────────────────────────────────────
        private readonly ReactiveProperty<bool> _isInCover = new(false);
        private readonly Subject<bool> _onFalseSafeMoundEntered = new();

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public ReadOnlyReactiveProperty<bool> IsInCover => _isInCover;

        /// <inheritdoc/>
        public Observable<bool> OnFalseSafeMoundEntered => _onFalseSafeMoundEntered;

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public CoverDetector2D() { }

        // ── ICoverDetector2D ───────────────────────────────────────
        /// <inheritdoc/>
        public void NotifyOverlapEnter(string coverTag, PlayerInputMode mode)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            if (coverTag == TagMoThuong)
            {
                SetCover(true);
            }
            else if (coverTag == TagFalseSafeMound)
            SetCover(true);
            _onFalseSafeMoundEntered.OnNext(true);
            // Unknown tags silently ignored
        }

        /// <inheritdoc/>
        public void NotifyOverlapExit(string coverTag, PlayerInputMode mode)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            if (coverTag == TagMoThuong || coverTag == TagFalseSafeMound)
            {
                SetCover(false);
            }
        }

        // ── Private Methods ────────────────────────────────────────
        private void SetCover(bool value)
        {
            if (_isInCover.Value != value)
            {
                _isInCover.Value = value;
            }
        }
    }
}
