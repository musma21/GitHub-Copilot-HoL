---
applyTo: "MockWinAppInstaller/**"
version: 1.0
description: "Base path-specific delta rules for MockWinAppInstaller (WPF MVVM firmware/update simulation)."
lastUpdated: 2025-11-05
---

# Path-Specific Instructions (Base): MockWinAppInstaller
> Layered above global `/.github/copilot-instructions.md`. Only core delta; specialized topics split into separate instruction files.

## Purpose
WPF MVVM mock firmware installer: simulate update progress, checksum validation, protocol selection (DNP / MODBUS), localization (EN/KR).

## Platform Constraints
- Target framework: `net8.0-windows` (UI requires Windows 10/11).
- macOS/Linux: build/restore possible (EnableWindowsTargeting) but WPF run unsupported.
- Prefer single-file publish (win-x64) unless multi-RID explicitly needed.

## Architectural Rules
- Business logic resides in ViewModels/Services (keep code-behind minimal).
- Use ICommand for user actions; avoid heavy event handler logic.
- Async update simulation with cancellation token; avoid Thread.Sleep blocking.

## Localization Basics
- Add English resource key first, then Korean counterpart.
- No concatenated fragments to form full phrases.
- Placeholders: use `{0}`, `{1}` (culture formatting deferred).

## Ask Before (Delta)
Adding Windows-only dependency beyond WPF core; introducing persistence (registry/files) outside transient simulation; expanding protocol list beyond DNP/MODBUS; using reflection/dynamic code-gen; starting multiple timers/tasks without a unified cancellation path.

## Commit Tag
Prefix scope-local changes with: `installer:` (e.g. `installer: add progress cancellation`).

Last updated: 2025-11-05
