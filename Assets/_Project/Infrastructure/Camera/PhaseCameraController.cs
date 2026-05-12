using Unity.Cinemachine;
using R3;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using VContainer;
namespace SolarPhobia.Infrastructure.Camera
{
    /// <summary>
    /// Manages switching between Day (fixed) and Night (follow) Cinemachine cameras
    /// based on the current game phase.
    /// </summary>
    public class PhaseCameraController : MonoBehaviour
    {
        [Header("Cinemachine Cameras")]
        [SerializeField] private CinemachineCamera _dayFixedCamera;
        [SerializeField] private CinemachineCamera _nightFollowCamera;

        [Header("Priority Settings")]
        [SerializeField] private int _activePriority = 10;
        [SerializeField] private int _inactivePriority = 0;

        private IPhaseStateMachine _phaseStateMachine;
        private readonly CompositeDisposable _subscriptions = new();

        [Inject]
        public void Construct(IPhaseStateMachine phaseStateMachine)
        {
            _phaseStateMachine = phaseStateMachine;
        }

        private void Start()
        {
            if (_phaseStateMachine == null)
            {
                Debug.LogWarning("[PhaseCameraController] PhaseStateMachine not injected!");
                return;
            }

            _phaseStateMachine.OnPhaseChanged
                .Subscribe(OnPhaseChanged)
                .AddTo(_subscriptions);

            // Initialize camera state based on current phase
            UpdateCameraPriorities(_phaseStateMachine.CurrentState);
        }

        private void OnDestroy()
        {
            _subscriptions.Dispose();
        }

        private void OnPhaseChanged(PhaseChangedEvent e)
        {
            UpdateCameraPriorities(e.NewPhase);
        }

        private void UpdateCameraPriorities(PhaseState phase)
        {
            bool isNight = IsNightPhase(phase);

            if (isNight)
            {
                SetPriority(_dayFixedCamera, _inactivePriority);
                SetPriority(_nightFollowCamera, _activePriority);
            }
            else
            {
                SetPriority(_dayFixedCamera, _activePriority);
                SetPriority(_nightFollowCamera, _inactivePriority);
            }
        }

        private bool IsNightPhase(PhaseState phase)
        {
            return phase == PhaseState.NightTravel || 
                   phase == PhaseState.NightSurvival || 
                   phase == PhaseState.ShrineArrival;
        }

        private void SetPriority(CinemachineCamera cam, int priority)
        {
            if (cam != null)
            {
                cam.Priority.Value = priority;
            }
        }
    }
}

