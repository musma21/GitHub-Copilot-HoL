---
applyTo: "(ANNOTATED SAMPLE – not active)"
description: "Annotated example of path-specific instructions for MockWinAppInstaller; explains each section and relation to specialized instruction files."
---

 
# Sample Path-Specific Instructions (Annotated) – MockWinAppInstaller

> This file is a teaching aid. The authoritative active base file lives at `.github/instructions/MockWinAppInstaller.instructions.md`. Do not copy blockquote comments into the real file; keep production instructions concise.

---

## Front Matter

 
```yaml
applyTo: "MockWinAppInstaller/**"
description: "Base path-specific delta rules for MockWinAppInstaller (WPF MVVM firmware/update simulation)."
```
 
> Why `applyTo` matters: It scopes these rules only to files under `MockWinAppInstaller/`, preventing unrelated folders from inheriting specialized MVVM or firmware simulation constraints. Without this pattern Copilot treats the content as generic and may over-apply it. Only `applyTo` and `description` are supported keys—`version` or `lastUpdated` were removed to stay spec-compliant.
> Relation to other instruction files: This base file is complemented by more focused modules (`MockWinAppInstaller.testing.instructions.md`, `MockWinAppInstaller.pitfalls.instructions.md`). The base stays minimal; deeper guidance moves to specialized files to reduce token usage and cognitive load.

## Purpose

WPF MVVM mock firmware installer: simulate update progress, checksum validation, protocol selection (DNP / MODBUS), localization (EN/KR).
> Defines the functional domain and ensures generated code aligns with simulation goals instead of real hardware integration.

## Platform Constraints

- Target framework: `net8.0-windows` (UI requires Windows 10/11).
- macOS/Linux: build/restore possible (EnableWindowsTargeting) but WPF run unsupported.
- Prefer single-file publish (win-x64) unless multi-RID explicitly needed.

> Clarifies cross-platform limits so Copilot doesn’t propose non-Windows runtime paths for WPF UI or unnecessary multi-RID packaging.

## Architectural Rules

- Business logic resides in ViewModels/Services (keep code-behind minimal).
- Use ICommand for user actions; avoid heavy event handler logic.
- Async update simulation with cancellation token; avoid Thread.Sleep blocking.

> Enforces MVVM separation and encourages testable, cancellable async flows rather than blocking UI thread calls.

## Localization Basics

- Add English resource key first, then Korean counterpart.
- No concatenated fragments to form full phrases.
- Placeholders: use `{0}`, `{1}` (culture formatting deferred).

> Prevents broken localized strings (fragment concat) and ensures English remains the authoritative base for parity checks.

## Ask Before (Delta)

Adding Windows-only dependency beyond WPF core; introducing persistence (registry/files) outside transient simulation; expanding protocol list beyond DNP/MODBUS; using reflection/dynamic code-gen; starting multiple timers/tasks without a unified cancellation path.
> These are potential scope or complexity escalators. Copilot should pause, request confirmation, and not silently introduce them.

## Commit Tag

Prefix scope-local changes with: `installer:` (e.g. `installer: add progress cancellation`).
> Enables quick filtering of commits affecting this subsystem and improves review traceability.

## Relationship to Specialized Instruction Files

- Testing: `.github/instructions/MockWinAppInstaller.testing.instructions.md` → defines minimum coverage & test principles.
- Pitfalls: `.github/instructions/MockWinAppInstaller.pitfalls.instructions.md` → lists recurring errors and mitigations.
- Coding Conventions: `.github/instructions/MockWinAppInstaller.codeing-convention-instructions.md` → condensed C# style & patterns referencing official Microsoft guide.

> Keeping these separate keeps the base lean and reduces repetition; future modules (e.g. `refactor`, `deviation`) can be added similarly. Coding conventions file prevents style noise from bloating functional rules.

## Last Updated Annotation

Last updated (base real file): 2025-11-05
> Timestamp is informative here only; real instruction files avoid unsupported front matter keys.

---
> End of annotated sample. For production usage reference only the concise base and specialized files.
