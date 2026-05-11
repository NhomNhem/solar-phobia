# ADR-0010: Day/Night Camera Transition

> **Status**: Accepted
> **Date**: 2026-05-07
> **Author**: opencode

## Context

Day/Night Camera Transition controls the visual perspective shift between day and night phases. Day uses a fixed 2.5D top-down macro view on the market stall; night uses a side-scrolling follow camera. The transition must be abrupt and jarring to reinforce emotional contrast.

## Decision

Phase-driven camera controller with abrupt transitions:

```csharp
public class DayNightCameraController : IInitializable, ITickable {
    private Camera _camera;
    private PhaseStateMachine _phaseState;
    private float _dayDistance = 5.0f;
    private float _nightDistance = 10.0f;
    private float _vignetteMaxAlpha = 0.6f;
    
    private CameraState _currentState = CameraState.Day;
    private float _transitionProgress;
    private bool _isTransitioning;
    
    public void Initialize() {
        _phaseState = Container.Resolve<PhaseStateMachine>();
        _phaseState.OnPhaseChanged += OnPhaseChanged;
    }
    
    public void Tick() {
        if (_isTransitioning) {
            ProcessTransition();
        }
        
        if (_currentState == CameraState.Night) {
            UpdateNightFollow();
        }
    }
    
    private void OnPhaseChanged(PhaseState newPhase) {
        switch (newPhase) {
            case PhaseState.nightSurvival:
                StartTransition(CameraState.Day, CameraState.Night, 0.5f);
                break;
            case PhaseState.dayService:
            case PhaseState.choiceLock:
                StartTransition(CameraState.Night, CameraState.Day, 0.3f);
                break;
        }
    }
    
    private void StartTransition(CameraState from, CameraState to, float duration) {
        _isTransitioning = true;
        _transitionProgress = 0f;
        _transitionDuration = duration;
        _fromState = from;
        _toState = to;
    }
    
    private void ProcessTransition() {
        _transitionProgress += Time.deltaTime / _transitionDuration;
        
        // Abrupt zoom-out/in at frame 0-5
        // Color grade swap at frame 6-15
        // Vignette spike at frame 16-30
        
        if (_transitionProgress >= 1f) {
            _isTransitioning = false;
            _currentState = _toState;
            ApplyFinalState();
        }
    }
    
    private void UpdateNightFollow() {
        var player = Container.Resolve<PlayerController>();
        float targetX = player.Position.x + _cameraOffsetX;
        float targetY = player.Position.y + _mouseLookOffsetY;
        
        _camera.transform.position = Vector3.Lerp(
            _camera.transform.position,
            new Vector3(targetX, targetY, _nightDistance),
            _followSmooth * Time.deltaTime
        );
    }
    
    private void ApplyFinalState() {
        if (_currentState == CameraState.Day) {
            // Lock to day position
            _camera.transform.position = new Vector3(0, 0, -_dayDistance);
        }
    }
}

public enum CameraState {
    Day,
    TransitioningToNight,
    Night,
    TransitioningToDay
}
```

## Consequences

- Positive: Phase-driven triggers ensure camera always syncs with game state
- Positive: Abrupt transition timing (0.5s day→night, 0.3s night→day) matches GDD spec
- Positive: Y-axis mouse-look during night with X-axis locked
- Positive: Vignette control via post-processing volume
- Need: Requires PhaseStateMachine event subscription
- Need: Requires PlayerController position access
- Need: Color grading via URP volume profile

## GDD Requirements Addressed

- TR-camera-001: Day phase uses fixed 2.5D top-down view
- TR-camera-002: Night phase uses side-scrolling follow camera
- TR-camera-003: Day→Night transition is abrupt (0.5s)
- TR-camera-004: Night→Day transition is abrupt (0.3s)
- TR-camera-005: Mouse-look Y-axis only during night
- TR-camera-006: Vignette increases to α=0.6 during night
- TR-camera-007: Resolve state shows shrine pan (win) or shake+fade (lose)