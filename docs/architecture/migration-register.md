# Migration Register

Source of truth for the Option B folder migration. This register tracks where each current file lives,
where it should end up, and which batch will move it.

## Rules

- Feature-specific code must move under `Assets/_Project/<Layer>/Features/<Feature>/...`.
- `Application/Services` is deprecated for feature code and will be emptied over time.
- Namespace changes must mirror the target folder path 1:1.
- Do not change comment text or XML summaries unless the code change requires it.

## Batch 1 - Application feature buckets

| Current Path | Target Path | Target Namespace | Status | Notes |
|---|---|---|---|---|
| `Assets/_Project/Application/Day/DayActionController.cs` | `Assets/_Project/Application/Features/Day/DayActionController.cs` | `SolarPhobia.Application.Features.Day` | done | Day input actions controller |
| `Assets/_Project/Application/Day/IDayActionController.cs` | `Assets/_Project/Application/Features/Day/IDayActionController.cs` | `SolarPhobia.Application.Features.Day` | done | Day input contract |
| `Assets/_Project/Application/Day/DaySelectionValidator.cs` | `Assets/_Project/Application/Features/Day/DaySelectionValidator.cs` | `SolarPhobia.Application.Features.Day` | done | Day selection validation rule |
| `Assets/_Project/Application/Day/IDaySelectionValidator.cs` | `Assets/_Project/Application/Features/Day/IDaySelectionValidator.cs` | `SolarPhobia.Application.Features.Day` | done | Day selection validation contract |
| `Assets/_Project/Application/Day/DayServiceUIController.cs` | `Assets/_Project/Application/Features/Day/DayServiceUIController.cs` | `SolarPhobia.Application.Features.Day` | done | Day service UI orchestration |
| `Assets/_Project/Application/Day/IDayServiceUIController.cs` | `Assets/_Project/Application/Features/Day/IDayServiceUIController.cs` | `SolarPhobia.Application.Features.Day` | done | Day service UI contract |
| `Assets/_Project/Application/Strike/StrikeController.cs` | `Assets/_Project/Application/Features/Strike/StrikeController.cs` | `SolarPhobia.Application.Features.Strike` | done | Strike telegraph controller |
| `Assets/_Project/Application/Strike/IStrikeController.cs` | `Assets/_Project/Application/Features/Strike/IStrikeController.cs` | `SolarPhobia.Application.Features.Strike` | done | Strike controller contract |
| `Assets/_Project/Application/Ward/IWardTimerPort.cs` | `Assets/_Project/Application/Features/Ward/IWardTimerPort.cs` | `SolarPhobia.Application.Features.Ward` | done | Ward application port |

## Batch 2 - Remaining Application buckets

| Current Path Group | Target Path Group | Status | Notes |
|---|---|---|---|
| `Assets/_Project/Application/Flow/*` | `Assets/_Project/Application/Features/Phase/Flow/*` | done | Flow coordination belongs to Phase feature |
| `Assets/_Project/Application/Phase/*` | `Assets/_Project/Application/Features/Phase/*` | done | Phase flow, day timeline, reset logic |
| `Assets/_Project/Application/Player/*` | `Assets/_Project/Application/Features/Player/*` | done | Player input, movement, cursor, warnings, state, interactions |
| `Assets/_Project/Application/Audio/*` | `Assets/_Project/Application/Features/Audio/*` | done | Application-level audio cue port |
| `Assets/_Project/Application/Map/*` | `Assets/_Project/Application/Features/Map/*` | pending | Map analysis, cover, generation, spawn director |
| `Assets/_Project/Application/MainMenu/*` | `Assets/_Project/Application/Features/MainMenu/*` | pending | Main menu application services |
| `Assets/_Project/Application/Resources/*` | `Assets/_Project/Application/Features/Resources/*` | pending | Resource effects and NgocCot resource feature |
| `Assets/_Project/Application/Rituals/*` | `Assets/_Project/Application/Features/Rituals/*` | pending | Ritual assignment feature |
| `Assets/_Project/Application/Shrines/*` | `Assets/_Project/Application/Features/Shrines/*` | pending | Shrine objective feature |
| `Assets/_Project/Application/Repositories/Soul.cs` | `Assets/_Project/Domain/Entities/Soul.cs` | `SolarPhobia.Domain` | done | Soul entity belongs in Domain |
| `Assets/_Project/Application/Repositories/ISoulRepository.cs` | `Assets/_Project/Domain/Repositories/ISoulRepository.cs` | `SolarPhobia.Domain.Repositories` | done | Domain repository contract |
| `Assets/_Project/Application/Repositories/SoulRepository.cs` | `Assets/_Project/Infrastructure/Features/Soul/SoulRepository.cs` | `SolarPhobia.Infrastructure.Features.Soul` | done | In-memory soul repository implementation |
| `Assets/_Project/Application/Messages/SelectionChangedEvent.cs` | `Assets/_Project/Domain/Events/SelectionChangedEvent.cs` | `SolarPhobia.Domain.Events` | done | Domain event for soul selection changes |
| `Assets/_Project/Application/UseCases/*` | `Assets/_Project/Application/Features/*/UseCases/*` | done | Use cases redistributed by feature |
| `Assets/_Project/Application/Messages/*` | `Assets/_Project/Application/Messages/*` | pending | Cross-cutting contracts stay in place for now |
| `Assets/_Project/Infrastructure/Features/Ghost/GhostRepository.cs` | `Assets/_Project/Infrastructure/Features/Ghost/GhostRepository.cs` | `SolarPhobia.Infrastructure.Features.Ghost` | done | In-memory ghost repository for combat/consequence flows |

## Batch 3 - Infrastructure and Presentation cleanup

| Current Path Group | Target Path Group | Status | Notes |
|---|---|---|---|
| `Assets/_Project/Infrastructure/Audio/*` | `Assets/_Project/Infrastructure/Features/Audio/*` | pending | Audio adapters and services |
| `Assets/_Project/Infrastructure/Camera/*` | `Assets/_Project/Infrastructure/Features/CameraControl/*` | pending | Camera runtime adapters |
| `Assets/_Project/Infrastructure/Dialogue/*` | `Assets/_Project/Infrastructure/Features/Dialogue/*` | pending | Dialogue persistence/runtime |
| `Assets/_Project/Infrastructure/Hazards/*` | `Assets/_Project/Infrastructure/Features/Hazards/*` | pending | Hazard runtime adapters |
| `Assets/_Project/Infrastructure/Input/*` | `Assets/_Project/Infrastructure/Features/Input/*` | pending | Input runtime adapters |
| `Assets/_Project/Infrastructure/MainMenu/*` | `Assets/_Project/Infrastructure/Features/MainMenu/*` | pending | Main menu platform adapters |
| `Assets/_Project/Presentation/HUD/*` | `Assets/_Project/Presentation/Features/HUD/*` | pending | HUD views and Toolkit UI |
| `Assets/_Project/Presentation/MainMenu/*` | `Assets/_Project/Presentation/Features/MainMenu/*` | pending | Main menu views and controllers |
| `Assets/_Project/Presentation/Player/*` | `Assets/_Project/Presentation/Features/Player/*` | pending | Player views and controllers |
