# Customizing GitHub Copilot Responses (English Version)

> For extended background, original examples, and legacy formatting see `customizing-GHCP-response-extended.md`. Below are the instruction assets used in this Hands-on Lab:
>
> - Repository-wide: `.github/copilot-instructions.md`
> - Path-specific: `.github/instructions/MockWinAppInstaller.instructions.md`, `.github/instructions/MockWinAppInstaller.testing.instructions.md`, `.github/instructions/MockWinAppInstaller.pitfalls.instructions.md`, `.github/instructions/MockWinAppInstaller.codeing-convention-instructions.md`
> - Agents: `AGENTS.md`
> - Prompt files location: `.github/prompts/`
>
> If a longer (15+ line) instruction feels unavoidable, split it by intent into smaller imperative bullets (e.g. instead of one "Test strategy" paragraph: "Include boundary values", "Add at least one negative case", "Cover async cancellation paths").

## Quick Summary

- Four customization channels: Repository Instructions / Path-Specific / Agent Instructions / Prompt Files.
- Goal: Reduce repetitive clarifications; enforce architecture, coding and testing rules consistently.
- Maintainability: concise (≤600 words repository-wide), imperative style, no duplicated prose.

## 1. Types of Customization

| Type | Scope | Injection | Typical Use |
|------|-------|-----------|-------------|
| Repository Instructions (`.github/copilot-instructions.md`) | Whole codebase | Automatic | Architecture, build, forbidden patterns |
| Path-Specific (`.github/instructions/*.instructions.md`) | Directory subtree | Automatic (higher precedence) | Module rules, test focus |
| Agent Instructions (`AGENTS.md`) | Behavior / persona | Automatic (VSCode Chat settings) | git-mini, code-mini role response shaping |
| Prompt Files (`*.prompt.md`) | Reusable per-task lines | Manual selection | Repeated refactors, test generation |

> Prompt Files (VSCode/JetBrains – Public Preview). Invoke via Command Palette or `/filename` in chat.

## 2. Repository Instructions

Authoring principles:

- First 3–6 lines: languages, frameworks, build system, primary goal.
- Core commands: install → build → test (verified order).
- Use "Always" / "Must" prefix for non‑skippable steps.
- Declare forbidden patterns (e.g. blocking `.Result` / `.Wait`).

Configuration:

1. Add `.github/copilot-instructions.md`.
2. Enable `github.copilot.chat.codeGeneration.useInstructionFiles` in VSCode.
3. Reference in chat with `#file:.github/copilot-instructions.md`.

## 3. Path-Specific Instructions

Purpose: overlay special rules for submodules (e.g. WPF MVVM, test requirements).
Example structure:

```text
.github/instructions/
  MockWinAppInstaller.instructions.md          # baseline
  MockWinAppInstaller.testing.instructions.md  # test strategy
  MockWinAppInstaller.pitfalls.instructions.md # recurring mistakes
  MockWinAppInstaller.coding-convention.instructions.md
```
Precedence: Path-Specific > Repository-wide (within that path).

## 4. Agent Instructions (AGENTS.md)

Configuration:

- VSCode settings: `chat.useAgentsMdFile`, `chat.useNestedAgentsMdFiles`.
- Root `AGENTS.md` (nested files optional with preview feature).

Minimal example:

```markdown
Agent: git-mini
Scope: core git verbs & single-conflict fix. Behavior: terse 1–2 bash lines.

Agent: term-mini
Scope: glossary/acronym ≤3 sentences.

Agent: code-mini
Scope: micro code edits (≤15 changed lines, ≤2 files).

Agent: arch-pro
Scope: architecture, refactor, performance, security (structured output).
```

Usage: Mention `@git-mini`, `@code-mini` directly in chat.

## 5. Prompt Files

Location: default `.github/prompts/`; extend via `chat.promptFilesLocations`.
Workflow:

1. Editor play button.
2. Chat `/filename` autocomplete.
3. Command Palette: "Chat: Run Prompt".

Writing rules:

- One imperative task per line.
- Keep 15–30 lines (prune if >50).
- Length per line ≤160 chars (≤100 Korean chars if bilingual).

Example:

```text
Refactor #file:src/Services/ChecksumService.cs to use async hashing with cancellation.
Add tests for #folder:src/ViewModels focusing progress cancellation edge cases.
Explain DNP vs MODBUS protocol choice for firmware update (trade-offs).
```

## 6. Syncing VSCode Settings Across Devices

VSCode Settings Sync keeps instruction/prompt settings aligned.

![Settings Sync enable screen](assets/settings-sync-enable.png "Settings Sync enable screen")

## 7. Best Practices

### What Goes Where

| Put in Instructions | Put in Prompt File |
|---------------------|--------------------|
| Rules / architecture / mandatory build steps | Repeated executable requests (refactor, add tests) |
| Forbidden libraries / patterns | File-scoped change directives |
| Project directory overview | Frequent analysis or validation commands |
| Test coverage focus | Single-purpose executable tasks |

### Length Guidelines

- Repo instructions: ≤600 English words or ≤2,000 Korean characters.
- Prompt file: small, scan-friendly collection.

### Style

- Short, imperative: "Prefer async/await; avoid blocking waits.".
- Neutral technical tone by default; adapt only on explicit user request.
- Avoid meta-prose (“This document will…”).

### Maintenance Checklist

- [ ] Links valid (no 404)
- [ ] Image alt text present
- [ ] Typos removed (`prompth`, `consisitency` → fixed)
- [ ] Forbidden items updated
- [ ] Prompt file line count ≤30
- [ ] No stray HTML entities (`&#x20;` etc.)

## 8. Examples (Good vs Improved)

### Good (Before)

“React components: function components only (no class).”

### Improved Pattern


```text
React: function components only.
Imports: use absolute path (no ../../../).
Async: use async/await; avoid raw Promise chains.
```

### Bad → Reason → Rephrase

| Bad | Problem | Rephrase |
|-----|---------|----------|
| “Answer all questions in informal style.” | Tone hard-coded | “Default concise technical tone; switch to casual only if user asks.” |
| “Use @terminal for Git.” | No trigger condition | “If user asks for a command (contains ‘run’/‘command’), show Git CLI.” |
| “Always conform to styleguide.md in my-repo.” | Too vague & huge surface | Split into precise bullets (see improved pattern). |

## 9. Common Pitfalls (Copilot Context)

| Pitfall | Mitigation |
|---------|------------|
| Instruction overly long → key points lost | Enforce length boundaries + prioritize essentials |
| Paragraph-style prompt lines | Split into atomic single-line tasks |
| Dark theme screenshots reduce contrast | Re-capture in light theme + crop + highlight |
| Hashed image filenames meaningless | Replace with semantic descriptive names |

> Long instructions (15+ lines) degrade summarization and risk partial adherence. Convert them into multiple short bullets grouped by intent (setup, architecture, testing, security), enabling reliable parsing and reuse.

## 10. References

- Customizing Copilot responses  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/concepts/prompting/response-customization>
- Customization examples library  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/customization-library/custom-instructions>
- Best practices for using GitHub Copilot  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/coding-agent/get-the-best-results>
- Adding repository custom instructions  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/how-tos/configure-custom-instructions/add-repository-instructions>
- Soeun Park guide (Korean)  
  <https://cdn.microbiz.ai/public/GHE/github-copilot-instructions.md-guide.pdf>
