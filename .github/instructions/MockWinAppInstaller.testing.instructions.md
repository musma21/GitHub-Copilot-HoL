---
applyTo:
  - "MockWinAppInstaller/src/**/*.cs"
  - "MockWinAppInstaller/src/**/*.xaml"
description: "Testing guidance for MockWinAppInstaller path (minimal required coverage)."
---

# Testing Instructions: MockWinAppInstaller
> Supplements base path-specific rules. Keep tests fast, deterministic, and UI-thread safe.

## Minimum Coverage
- ChecksumService: valid match & mismatch.
- UpdateSimulator: progress increments & cancellation terminal state.
- MainViewModel: command CanExecute transitions & state changes.
- Optional: resource key parity (EN vs KO).

## Principles
- No blocking calls on UI thread; prefer async harness or dispatcher abstraction.
- Inject configurable delay for simulation (avoid hardcoded sleeps).
- Avoid fragile locale/time assertions; normalize formatting or mock clock.

## Suggested Patterns
- Arrange/Act/Assert with clear state setup for ViewModel.
- Use CancellationTokenSource with short timeout for simulator tests.
- Consider result object or enum for simulator terminal states for clarity.

Last updated: 2025-11-05
