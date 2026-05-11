# Unity 6 — Deprecated APIs

> Last verified: 2026-05-07

This document lists deprecated APIs and their recommended replacements.

---

## HIGH Priority (stop using)

| Deprecated API | Replacement | Since |
|----------------|-------------|-------|
| `Object.FindObjectsOfType()` | `Object.FindObjectsByType(FindObjectsSortMode.None)` | Unity 6.0 |
| `Object.FindObjectOfType<T>()` | `Object.FindObjectOfType<T>()` (still available) | Unity 6.0 |
| `RenderPipelineEditorUtility.FetchFirstCompatibleTypeUsingScriptableRenderPipelineExtension()` | `GetDerivedTypesSupportedOnCurrentPipeline()` | Unity 6.0 |
| `CustomEditorForRenderPipelineAttribute` | `CustomEditor(typeof(T), true)` + custom draw | Unity 6.0 |
| `VolumeComponentMenuForRenderPipelineAttribute` | Use `VolumeComponentMenu` | Unity 6.0 |
| `AccessibilityNode.selected` | `AccessibilityNode.invoked` | Unity 6.3 |
| `Netcode for 1.X` | Netcode for GameObjects 2.0+ | Unity 6.3 |
| `Multiplay Hosting` | Use Unity Gaming Services alternatives | Unity 6.3 |
| Legacy ETC compression | Default ETC compressor configuration | Unity 6.3 |

---

## MEDIUM Priority (plan to migrate)

| Deprecated API | Replacement | Since |
|----------------|-------------|-------|
| `UnityPlayer` (Android Java class) | `UnityPlayerForActivityOrService` | Unity 6.0 |
| IMGUI (OnGUI) | UI Toolkit | Unity 6.0+ |
| uGUI (legacy) | UI Toolkit | Unity 6.0+ |
| Universal Compatibility Mode | Standard rendering | Unity 6.3 |
| `Experimental.AdditionalBakedProbes` | `LightTransport.IProbeIntegrator` | Unity 6.3 |
| Facebook Instant Games | Not recommended | Unity 6.3 |

---

## Project-Specific Notes

Since this project uses:
- **VContainer** — DI pattern is correct, no deprecation concerns
- **R3** — Reactive system, no deprecation concerns  
- **New Input System** — This is the recommended approach
- **DOTween** — Third-party, check for Unity 6 compatibility

---

## Cross-Reference

- See `breaking-changes.md` for breaking changes
- See `current-best-practices.md` for recommended alternatives