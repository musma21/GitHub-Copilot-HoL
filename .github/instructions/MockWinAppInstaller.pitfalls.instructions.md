---
applyTo: "MockWinAppInstaller/**"
description: "Common pitfalls & mitigations for MockWinAppInstaller path."
---

# Pitfalls & Mitigations: MockWinAppInstaller
> Quick reference to avoid recurring mistakes.

| Pitfall | Mitigation |
|---------|------------|
| Missing DataContext → silent bindings | Set DataContext early (App startup or window ctor) |
| Multiple timers racing progress | Use single scheduler + CancellationToken |
| Resource key naming drift | Enforce consistent prefix (e.g. UpdateProgress*) |
| Mixing sync file IO with UI thread | Use async I/O or background tasks |
| Code-behind logic creep | Move logic into ViewModel ICommand |
| Hardcoded sleep in simulator | Inject delay config; await Task.Delay |
| Localization fragment concatenation | Add full phrase key; no string assembly |

Last updated: 2025-11-05
