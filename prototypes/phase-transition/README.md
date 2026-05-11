# Prototype: Phase Transition

## Purpose
Test whether the day→night transition creates the intended **emotional whiplash** — feeling thrown from a cramped, safe space (day) into an open, dangerous void (night).

## Core Question
> Does the abrupt camera zoom-out + color grade swap + vignette spike make the player feel "the world suddenly got bigger and more dangerous"?

## Setup Instructions

1. **Create New Scene** in Unity
2. **Main Camera**: Add to scene, reset transform
3. **Create UI** (Canvas):
   - Text (Status): top-center, large font
   - Text (Timer): below status
   - Button (Trigger): bottom-center, "Trigger Night"
   - Image (Vignette): full-screen black, alpha 0, raycast target OFF
   - RawImage (ColorGrade): full-screen, raycast target OFF
4. **Create Empty GameObject** "PhaseController"
5. Add `PhaseTransitionController.cs` to it
6. **Link references** in Inspector:
   - Status Text → UI Text
   - Timer Text → UI Text
   - Trigger Button → UI Button
   - Vignette Overlay → UI Image (black)
   - Color Grade Overlay → UI RawImage
7. **Press Play** — day phase starts automatically (5s timer)
8. **Or click button** to trigger transition manually

## What to Observe

| Observation | Expected | Concern |
|-------------|----------|---------|
| Camera zoom | Abrupt pull-back, not smooth | If smooth → too gentle |
| Color swap | Immediate warm→cold shift | If gradual → not jarring enough |
| Vignette | Sharp increase to 60% | If subtle → not oppressive |
| Overall feel | "Thrown into darkness" | If "transition" → not dramatic enough |

## Manual Test Checklist

- [ ] Day phase feels confined (close camera)
- [ ] Transition feels abrupt (0.5s, not 2s)
- [ ] Night feels open (pulled-back camera)
- [ ] Cold color grade visible (blue tint)
- [ ] Vignette creates claustrophobia (dark edges)
- [ ] Transition back to day feels different (faster, warmer)

## Tech Stack Used
- Unity 6000.3.11f1
- No external packages needed (prototype uses basic Unity features)

---
*Prototype - Not for Production*