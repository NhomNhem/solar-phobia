## Technical Debt Register
Last updated: 2026-05-11
Total items: 4 | Estimated total effort: M

| ID | Category | Description | Files | Effort | Impact | Priority | Added | Sprint |
|----|----------|-------------|-------|--------|--------|----------|-------|--------|
| TD-001 | Dependency | Fix Input System assembly reference in MainMenuController. Currently commented out. | `MainMenuController.cs` | S | Med | 10 | 2026-05-11 | Backlog |
| TD-002 | Code Quality | `IsValidTransition` exceeds 40-line limit in `PlayerStateMachine.cs`. | `PlayerStateMachine.cs` | S | Low | 5 | 2026-05-11 | Backlog |
| TD-003 | Architecture | OCP violation in `PlayerStateMachine.cs`: hardcoded switch for transitions. | `PlayerStateMachine.cs` | M | Med | 15 | 2026-05-11 | Backlog |
| TD-004 | Code Quality | `DayNightCameraController.cs` is approaching 500 lines and may have high complexity. | `DayNightCameraController.cs` | M | Med | 8 | 2026-05-11 | Backlog |
