# Story 003: Critical Sensory Feedback

> **Epic**: ward-timer
> **Status**: Ready
> **Layer**: Presentation
> **Type**: Visual/Feel
> **Estimate**: 0.5h

## Context

**GDD**: `design/gdd/health-stamina-damage-rules.md`
**Requirement**: When Ward drops to critical levels (≤25%), screen effects signal imminent death.

**ADR**: ADR-0005 (Ward Timer Implementation)
**Engine**: Unity 6000.3.11f1 | **Risk**: LOW

## Acceptance Criteria

- [ ] At ≤25% (Panic tier): chromatic aberration vignette, subtle audio distortion
- [ ] At ≤10s remaining (DeathSpiral tier): heavy chromatic aberration, audio distortion intensifies, screen edges darken
- [ ] Effects fade out smoothly when Ward resets (day transition)
- [ ] Audio: heartbeat bass swell at Panic, distorted low drone at DeathSpiral

## Implementation Notes

- Consume `WardTimerService.CurrentTier` observable
- Use Volume component for post-processing (chromatic aberration via URP)
- Audio via AudioMixer snapshot blending (use existing AudioStateDirector if available)
- DOTween for transition smoothness (0.5s fade between tiers)

## Test Evidence

**Type**: Visual/Feel
**Required**: Video/screenshot evidence in `production/qa/evidence/ward-sensory-feedback.md`
