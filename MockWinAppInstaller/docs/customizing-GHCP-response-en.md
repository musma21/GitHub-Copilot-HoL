# Customizing GitHub Copilot Responses (English)

> For detailed background and original examples see [customizing-GHCP-response-extended.md](customizing-GHCP-response-extended.md). Quick links used in this Hands‑on Lab (language versions: [KO](customizing-GHCP-response-ko.md) · [EN](customizing-GHCP-response-en.md))
> - Repository-wide: [`.github/copilot-instructions.md`](../../.github/copilot-instructions.md)
> - Path-specific: [`MockWinAppInstaller.instructions.md`](../../.github/instructions/MockWinAppInstaller.instructions.md), [`MockWinAppInstaller.testing.instructions.md`](../../.github/instructions/MockWinAppInstaller.testing.instructions.md), [`MockWinAppInstaller.pitfalls.instructions.md`](../../.github/instructions/MockWinAppInstaller.pitfalls.instructions.md), [`MockWinAppInstaller.codeing-convention-instructions.md`](../../.github/instructions/MockWinAppInstaller.codeing-convention-instructions.md)
> - Agents: [`AGENTS.md`](../../AGENTS.md)
> - Prompt files: [`.github/prompts/`](../../.github/prompts/)

Short summary:

- Four main instruction types + prompt files
  - Repository Instructions (global)
  - Path-Specific Instructions
  - Agent Instructions
  - Prompt Files
- Goal: provide context to Copilot to reduce repeated explanations and ensure consistency for architecture / rules / patterns.
- Readability & maintainability 
  - Concise (EN: ≤600 words, KO: ≤2,000 chars) – minimize token consumption
  - Clear (imperative style), remove duplication
> “Readability” here means from GH Copilot’s perspective; asking Copilot to shorten once it confirms understanding is fine.
> For onboarding or human handover, provide separate human‑oriented commentary docs if needed.
>
> Commentary docs:
>
> - [AGENTS-commentary.md](AGENTS-commentary.md)
> - [coding-conventions-commentary.md](coding-conventions-commentary.md)
> - [copilot-instructions-commentary.md](copilot-instructions-commentary.md)
> - [path-specific-instructions-commentary.md](path-specific-instructions-commentary.md)

> 💡 Organization-level instructions are also available (Preview as of Nov. 12, 2025)<br/>
> 👉 [Adding organization custom instructions for GitHub Copilot](https://docs.github.com/en/copilot/how-tos/configure-custom-instructions/add-organization-instructions)<br/>
> ⚙️ Org Owner: enable "Organization Custom Instruction" in user profile "Feature Preview"

## 1. Types of Customization

| Type | Scope | Injection | Typical Use |
|------|-------|-----------|-------------|
| Repository Instructions (`.github/copilot-instructions.md`) | Whole codebase | Automatic | Architecture, build, forbidden patterns |
| Path-Specific Instructions (`.github/instructions/*.instructions.md`) | Specific directory | Automatic (higher precedence) | Submodule rules, test policy |
| Agent Instructions (`AGENTS.md`) | Role / style | Automatic (VSCode Chat, needs setting) | Role response style (git-mini, code-mini, etc.) |
| Prompt Files (`*.prompt.md`) | Collection of per-request lines | Manual (select/reuse) | Repeated tasks (refactor, add tests) |

> - Prompt Files: VSCode/JetBrains only (Public Preview). Invoke via Command Palette or `/promptFileName` (not injected automatically)
> - Why are prompt files not also called “instructions”? 🤔 (Automatic injection difference!)

## 2. Repository Instructions

Authoring principles:

- First 3–6 lines: languages / frameworks / build / primary goals.
- Core commands: install → build → test order.
- Use “Always” / “Must” prefix for mandatory steps.
- Explicit forbidden/allowed patterns (e.g. “No blocking .Result / .Wait”).

Configuration:

- Add `.github/copilot-instructions.md` at root.
- VSCode setting: `⚙️github.copilot.chat.codeGeneration.useInstructionFiles` enable
  > vscode://settings/github.copilot.chat.codeGeneration.useInstructionFiles
- Reference in chat: `#file:.github/copilot-instructions.md` (manual injection method).

## 3. Path-Specific Instructions

Purpose: overlay special rules for submodules (e.g. WPF MVVM / test requirements).
Example structure:

```text
.github/instructions/
  MockWinAppInstaller.instructions.md          # baseline
  MockWinAppInstaller.testing.instructions.md  # test strategy
  MockWinAppInstaller.pitfalls.instructions.md # recurring issue prevention
  MockWinAppInstaller.coding-convention.instructions.md
```

- Priority: Path-Specific > Repository-Wide (inside that path).

## 4. Agent Instructions (AGENTS.md)

Configuration:

- VSCode: `chat.useAgentsMdFile`, `chat.useNestedAgentsMdFiles` enable.
  > vscode://settings/chat.useAgentsMdFile vscode://settings/chat.useNestedAgentsMdFiles
- Root `AGENTS.md` (nested path copies supported via Preview feature).

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

Usage: mention `@git-mini`, `@code-mini` etc. in chat with a command.
> `@git-mini cmtpsh`
> If `cmtpsh` is defined as “commit and push all local changes” in instructions, have it return only concise result output.

## 5. Prompt Files

Location: default `.github/prompts/`; extend via setting (`chat.promptFilesLocations`).
Usage flow (manual injection method):

1. Editor top ▶ button click.
2. Chat input `/filename` autocomplete.
3. Command Palette: “Chat: Run Prompt”.

Writing rules:

- One imperative task per line.
- Keep 15–30 lines; if >50 → prune/archive.
- Length per line ≤160 chars (Korean ≤100 chars).

Example:

```text
Refactor #file:src/Services/ChecksumService.cs to use async hashing with cancellation.
Add tests for #folder:src/ViewModels focusing progress cancellation edge cases.
Explain DNP vs MODBUS protocol choice for firmware update (trade-offs).
```

## 6. VSCode Settings Sync Across Devices

Enable VSCode Settings Sync → auto-sync instruction / prompt related settings (below “Backup and Sync Settings...” panel).

![VSCode Settings Sync UI](assets/settings-sync-enable.png "Settings Sync enable screen")

## 7. Best Practices

### Instructions vs Prompt Files

| Instructions | Prompt File |
|---------------------|--------------------|
| Rules / architecture / mandatory build steps | Repeated execution requests (refactor, test generation, etc.) |
| Forbidden libraries / patterns | Single file change directives |
| Project directory overview | Frequently used analysis/validation commands |
| Test scope strategy | Single-purpose runnable task |

### Length Guidelines

- Instructions: ≤600 English words OR ≤2,000 Korean chars.
- Prompt File: maintain a small, easy-to-scan collection.

### Style

- Short & imperative: “Prefer async/await; avoid blocking waits.”
- Neutral tone → adapt only on explicit user request.
- Copilot only needs to understand clearly; minimal wording acceptable.

### Maintenance Checklist

- [ ] Ask GH Copilot for feedback against best practices
- [ ] Have Copilot compress wording to what it still fully understands
- [ ] Ask Copilot to generate separate human-readable commentary docs if needed
- [ ] Ask Copilot to rephrase condensed instructions; human verifies understanding

## 8. Examples

### Bad → Reason → Rephrase

| Bad | Problem | Rephrase |
|-----|---------|----------|
| “Answer all questions in informal style.” | Forced tone → lowers accuracy | “Default: concise technical tone. Switch to casual only if user asks.” |
| “Use @terminal for Git.” | No condition; token waste | “If user asks for a command (contains ‘run’/‘command’), then show Git CLI.” |
| “Always conform to styleguide.md in my-repo.” | Excessive / unclear scope | Split into concrete bullets (like examples above). |

## 9. References

- [About customizing GitHub Copilot responses](https://docs.github.com/en/enterprise-cloud@latest/copilot/concepts/prompting/response-customization)
- [Curated collection of examples](https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/customization-library/custom-instructions)
- [Best practices for using GitHub Copilot](https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/coding-agent/get-the-best-results)
- [Adding repository custom instructions for GitHub Copilot](https://docs.github.com/en/enterprise-cloud@latest/copilot/how-tos/configure-custom-instructions/add-repository-instructions)
- [Copilot-Instruction 1-pager guide by Soeun Park@MS](https://cdn.microbiz.ai/public/GHE/github-copilot-instructions.md-guide.pdf) (Korean)
- [Adding repository custom instructions for GitHub Copilot](https://docs.github.com/en/enterprise-cloud@latest/copilot/how-tos/configure-custom-instructions/add-repository-instructions)
