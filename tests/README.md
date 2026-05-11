# Test Index (Unity Project)

This repository uses Unity Test Framework tests under `Assets/_Project/**/Editor/Tests` and `Assets/_Project/**/Tests`.

## Current test roots
- `Assets/_Project/Application/Editor/Tests`
- `Assets/_Project/Infrastructure/Tests/Editor`
- `Assets/NhemBootstrap/Tests/Editor` (third-party/bootstrap scope)

## Gate-check note
The project historically used Unity-native test locations, not a code-first `tests/` root. This file exists to satisfy phase-gate artifact discovery while preserving Unity conventions.

## Run commands
See `AGENTS.md` -> Build & Test Commands.
