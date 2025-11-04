# Hands-on Lab – Copilot Global Instructions
Version: 1.1 (2025-11-05) – Added test baseline, conflict resolution, refined significant change, encryption & localization clarifications.
## Overview
Multi-module HoL; each lab keeps its own NAME.instructions.md. This file = global baseline only.
## Scope & Priority
Tone consistency, safety, minimal deps. Priority: Security > Integrity > Clarity > Performance.
## Ask Before Proceeding
New external dep; port/env schema change; persistent store/DB; refactor >3 files OR public API change; move shared folders; history rewrite; change encryption patterns; relax security constraints.
## Avoid
Unrequested heavy frameworks; secrets in code/logs; big patterns (CQRS/Repo) w/o need; premature micro-opt; broad wildcard encryption.
## Security
Validate inputs; list affected files for auth/session changes; never output secrets; prefer git-crypt/GPG; highlight history rewrite impact; flag supply chain risk; if secret leaks: redact + rotate + note (never include rotated secret value in commit message). Prefer explicit encrypted filename list over broad wildcards in `.gitattributes`.
## Performance
Clarity first; stream large files; measure before caching; avoid speculative parallelism. Current codebase is small—defer optimization until a measured UI freeze or build/test slowdown.
## Prompting
User: intent + constraints + examples + versions. Assistant: decompose, incremental diffs, add minimal tests, restate constraints if drift.
## Workflow & Branching
main=stable; labNN-start/labNN-solution pairs; feat/<desc>, fix/<issue>; confirm before rewrite (force push breaks old clones).
## Anti-Patterns
Mass copy/paste; silent catch-all; deep nesting; static mutable state; docs drifting from code; blocking async (.Result).
## Author Checklist
Lab file; no plaintext sensitive assets; learning outcomes; diff rationale; build/test (include happy path + 1 edge + 1 failure where applicable); license compliance; git-crypt key backup & revocation cert; confirm encryption pattern changes were approved.
## Troubleshooting
Re-provide constraints if drift; ask user to rank priorities; propose upgrade path; restate active constraints after long guidance.
## Glossary
History purge=git rewrite; Attributes=.gitattributes; Spec=sensitive design doc; HoL=Hands-on Lab; Revocation cert=GPG invalidate doc; cmtpsh=commit and push all changes; gitpull=fetch and merge changes from remote. (Add new glossary entries only if reused ≥2 times.)
## Response Protocol
Significant = (filesTouched >3) OR public API change OR new external dep OR encryption pattern change OR sensitive spec edit. For significant: provide plan + impact + wait confirm; small diffs otherwise; summarize security steps; report Build/Lint/Test PASS/FAIL. Path-specific instructions may refine but never relax security; on contradiction assistant must flag and ask.
## Safety Reminder
Lab specifics live in each NAME.instructions.md. Localization additions: add English key first, forbid fragment concatenation, ensure parity before translating.
Last updated: 2025-11-04