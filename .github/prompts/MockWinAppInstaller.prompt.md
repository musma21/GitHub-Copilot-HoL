mode: agent
---
# MockWinAppInstaller Prompt (Demo / English)

Purpose:
- Show difference between "no project prompts" and "with instruction + agent prompts" in under a minute.

Core Rules (condensed):
- ViewModels must not call File/Directory/Path directly; use service abstractions.
- Avoid blocking sync waits (.Result/.Wait); use async + cancellation.
- Localization: add English key first, then Korean; no string fragments concatenated.
- Small surgical code change: @code-mini. Architectural/security/refactor decisions: @arch-pro.

Agent Selection:
| Scenario | Agent |
|----------|-------|
| Single git command | @git-mini |
| Terminology / acronym | @term-mini |
| Micro patch (≤15 changed lines) | @code-mini |
| Design / performance / security | @arch-pro |

Quick Prompt Examples:
@git-mini push current branch
@term-mini Define MVVM in one sentence
@code-mini Add CancellationToken to UpdateSimulator.StartAsync (patch only)
@arch-pro Evaluate moving checksum off UI thread (Problem/Options/Trade-offs/Recommendation + edge cases)

Firmware Upload Bad (no prompts expected outcome):
- Logic crammed in MainWindow code-behind
- Hardcoded Korean/English strings
- Direct File.ReadAllBytes + SHA256 inline

Firmware Upload Good (with prompts):
- IFirmwareFileService.SelectAsync() abstraction
- ViewModel exposes UploadCommand
- IChecksumService async call with cancellation
- Resources key (FirmwareUploadCompleted) used instead of literal text

Reproduce Demo:
1. Temporarily rename instruction files → run prompt: "Add firmware upload feature" (observe bad structure).
2. Restore instruction files → same prompt (no extra hints) → observe service + VM separation + localization.

Failure Recovery Hints:
- If answer verbose commands: "commands only"
- If design unstructured: request "structured answer"
- If micro patch escalates into design essay: switch agent (@arch-pro) or re-issue with @code-mini.

Security Quick Rules:
- If secret-like token appears, instruct removal + rotation; do not echo value.
- Encryption pattern changes require explicit approval (arch-pro path).

One-line Summary:
Prompt + instruction files act as a compressed context layer → fewer words, correct architecture.