# Hands-on Lab - Copilot Global Instructions (Annotated)

> Annotated expansion of the authoritative global instructions at `./.github/copilot-instructions.md`. Each section below adds a short rationale. Use the original for the concise <60 line baseline. This version normalizes ASCII characters for better portability.

## Overview
Multi-module HoL; each lab keeps its own NAME.instructions.md. This file = global baseline only.
> Scope boundary: per-lab details live elsewhere; avoids mixing global vs local rules.

## Scope & Priority
Tone consistency, safety, minimal deps. Priority: Security > Integrity > Clarity > Performance.
> Decision order when trade-offs arise (e.g., clarity vs optimization).

## Ask Before Proceeding
New external dep; port/env schema change; persistent store/DB; refactor >3 files OR public API change; move shared folders; history rewrite.
> Triggers that need explicit confirmation to avoid silent large-impact changes.

## Avoid
Unrequested heavy frameworks; secrets in code/logs; big patterns (CQRS/Repo) w/o need; premature micro-opt; broad wildcard encryption.
> Minimizes complexity/attack surface; prevents scope creep and accidental over-encryption.

## Security
Validate inputs; list affected files for auth/session changes; never output secrets; prefer git-crypt/GPG; highlight history rewrite impact; flag supply chain risk; if secret leaks: redact + rotate + note.
> Baseline defensive posture + post-incident response steps.

## Performance
Clarity first; stream large files; measure before caching; avoid speculative parallelism.
> Evidence-based optimization to prevent premature complexity.

## Prompting
User: intent + constraints + examples + versions. Assistant: decompose, incremental diffs, add minimal tests, restate constraints if drift.
> Collaborative prompt protocol keeps generation focused and auditable.

## Workflow & Branching
main=stable; labNN-start/labNN-solution pairs; feat/<desc>, fix/<issue>; confirm before rewrite (force push breaks old clones).
> Branch taxonomy + force-push impact warning.

## Testing
Happy path + edge + one failure/exception; fast isolated; WPF ViewModel: test state w/o UI; avoid locale/time fragility.
> Minimal yet meaningful coverage including one negative scenario.

## CI Gates
All merges: build + tests (+ lint if configured) must PASS.
> Quality gate prevents regressions entering main.

## Commits
Format: <type>(<scope>): <imperative>; body = rationale + risks. Group related changes; reference lab number; explain encryption rule edits.
> Traceability and structured metadata for code review.

## Localization
Strings in .resx; English base then ko; avoid fragment concat; verify all base keys exist in other locales.
> Prevents i18n drift and broken concatenated phrases; encourages key parity checks.

## Anti-Patterns
Mass copy/paste; silent catch-all; deep nesting; static mutable state; docs drifting from code; blocking async (.Result).
> Common maintainability and correctness hazards.

## Author Checklist
Lab file; no plaintext sensitive assets; learning outcomes; diff rationale; build/test; license compliance; git-crypt key backup & revocation cert.
> Pre-publish gate: security + pedagogical completeness.

## Troubleshooting
Re-provide constraints if drift; ask user to rank priorities; propose upgrade path; restate active constraints after long guidance.
> Recovery steps when conversation or scope diverges.

## Glossary
History purge=git rewrite; Attributes=.gitattributes; Spec=sensitive design doc; HoL=Hands-on Lab; Revocation cert=GPG invalidate doc; cmtpsh=commit and push all changes.
> Central shorthand reduces repeated explanations.

## Response Protocol
Significant= >3 files OR public API OR new external dep. For significant: plan + impact + wait confirm; small diffs otherwise; summarize security steps; report Build/Lint/Test PASS/FAIL.
> Distinguishes major vs incremental change + mandates impact + quality status.

## Safety Reminder
Lab specifics live in each NAME.instructions.md.
> Separation of global vs lab-local rules reduces accidental conflation.

Last updated: 2025-11-04
> Timestamp aids auditability and prompts periodic review.
