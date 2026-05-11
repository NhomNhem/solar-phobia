# Story 002: Ward Countdown HUD Display

> **Epic**: ward-timer
> **Status**: Ready
> **Layer**: Presentation
> **Type**: UI
> **Estimate**: 1.0h

## Context

**GDD**: `design/gdd/health-stamina-damage-rules.md`
**Requirement**: Player must be able to see remaining Ward time during NightSurvival.

**ADR**: ADR-0005 (Ward Timer Implementation)
**Engine**: Unity 6000.3.11f1 | **Risk**: LOW

## Acceptance Criteria

- [ ] Ward countdown displayed as numeric timer (MM:SS format) on HUD during NightSurvival
- [ ] Timer hides outside NightSurvival phase
- [ ] Color changes at sensory tier thresholds (>75% white, >50% yellow, >25% orange, ≤25% red)
- [ ] At ≤25% (Panic tier), timer pulses/shakes gently
- [ ] At ≤10s remaining (DeathSpiral tier), timer turns deep red and pulses rapidly

## Implementation Notes

- UI Toolkit element in Night HUD
- Subscribe to `IWardTimerService.CurrentWardObservable` + `CurrentTier`
- Format: `_wardTimer.CurrentWard.ToString("F0")` or `TimeSpan.FromSeconds(ward).ToString(@"mm\:ss")`
- Use USS classes for color transitions per tier
- DOTween for pulse animation at critical tiers

## Test Evidence

**Type**: UI
**Required**: Screenshot evidence in `production/qa/evidence/ward-countdown-hud.md`
