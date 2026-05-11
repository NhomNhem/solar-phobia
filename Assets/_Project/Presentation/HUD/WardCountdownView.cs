using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain;
using SolarPhobia.Domain.ValueObjects;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Presentation.HUD
{
    public class WardCountdownView : MonoBehaviour
    {
        [Inject] internal SolarPhobia.Domain.IWardTimerService _wardTimer;
        [Inject] internal IPhaseStateMachine _phaseStateMachine;

        private UIDocument _document;
        private Label _timerLabel;
        private VisualElement _barFill;
        private VisualElement _root;
        private IDisposable _wardSubscription;
        private IDisposable _tierSubscription;
        private IDisposable _phaseSubscription;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            _root = _document.rootVisualElement;
            _timerLabel = _root.Q<Label>("ward-timer-label");
            _barFill = _root.Q<VisualElement>("ward-bar-fill");

            _root.style.display = DisplayStyle.None;
        }

        private void Start()
        {
            _wardSubscription = _wardTimer.CurrentWardObservable
            .Subscribe(OnWardChanged);

            _tierSubscription = _wardTimer.CurrentTier
            .Subscribe(OnTierChanged);

            _phaseSubscription = _phaseStateMachine.CurrentPhase
            .Subscribe(OnPhaseChanged);
        }

        private void OnDestroy()
        {
            _wardSubscription?.Dispose();
            _tierSubscription?.Dispose();
            _phaseSubscription?.Dispose();
        }

        private void OnPhaseChanged(PhaseState phase)
        {
            _root.style.display = phase == PhaseState.NightSurvival
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        }

        private void OnWardChanged(float wardValue)
        {
            int seconds = Mathf.CeilToInt(Mathf.Max(0f, wardValue));
            _timerLabel.text = $"{seconds / 60:D2}:{seconds % 60:D2}";

            if (_wardTimer.MaxWard > 0f)
            {
                float fill = Mathf.Clamp01(wardValue / _wardTimer.MaxWard);
                _barFill.style.width = new Length(fill * 100f, LengthUnit.Percent);
            }
        }

        private void OnTierChanged(SensoryTier tier)
        {
            _root.RemoveFromClassList("ward-tier-stable");
            _root.RemoveFromClassList("ward-tier-creeping-dread");
            _root.RemoveFromClassList("ward-tier-heavy-burden");
            _root.RemoveFromClassList("ward-tier-panic");
            _root.RemoveFromClassList("ward-tier-death-spiral");

            string tierClass = tier switch
            {
                SensoryTier.Stable => "ward-tier-stable",
                SensoryTier.CreepingDread => "ward-tier-creeping-dread",
                SensoryTier.HeavyBurden => "ward-tier-heavy-burden",
                SensoryTier.Panic => "ward-tier-panic",
                SensoryTier.DeathSpiral => "ward-tier-death-spiral",
                _ => "ward-tier-stable"
            };

            _root.AddToClassList(tierClass);
        }
    }
}
