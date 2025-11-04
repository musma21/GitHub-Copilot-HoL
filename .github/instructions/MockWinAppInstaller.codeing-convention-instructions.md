applyTo: "MockWinAppInstaller/src/**"
description: "Minimal C# + WPF MVVM coding conventions (delta over global)."
---
applyTo: "MockWinAppInstaller/src/**"
description: "Minimal C# + WPF MVVM coding conventions (delta over global)."
---

# Coding Conventions: MockWinAppInstaller (Minimal)
> Delta only; relies on global security/workflow. Keep logic testable & UI thin.

## Naming
MUST PascalCase (types/public); camelCase private fields; I* for interfaces; Async suffix; cancellationToken param name.

## Namespaces & Files
MUST file-scoped namespace; ONE public class per file; file name matches type; AVOID mixing styles.

## Usings
MUST at top outside namespace; SHOULD order System*, third-party, local; REMOVE unused.

## Layout
MUST 4 spaces (no tabs); CONSISTENT brace style; SHOULD methods ≤50 LOC; AVOID >120 column lines.

## Properties & Fields
MUST prefer auto-properties; backing field only for logic; SHOULD init-only/record for immutable config; AVOID complex getters.

## Null & Guards
MUST early guard (ArgumentNullException); RETURN early to avoid deep nesting.

## Async
MUST await (no .Result/.Wait()); MUST propagate cancellationToken; SHOULD ConfigureAwait(false) only in libraries; AVOID fire-and-forget without handling.

## Exceptions
MUST throw specific types; AVOID empty catch; SHOULD wrap to add context when rethrowing.

## LINQ & Collections
MUST Any() over Count()>0; SHOULD break long chains; SHOULD `ObservableCollection<T>` only if UI needs; AVOID repeated enumeration.

## Strings & Localization
MUST interpolate; MUST localize user-visible text (EN key first then KO); AVOID concatenated phrase fragments.

## Comments
SHOULD XML docs for public/complex logic; TODO with issue ref; AVOID large commented blocks.

## MVVM Commands
MUST read-only ICommand props; MUST keep business logic out of code-behind; SHOULD single SetProperty helper; AVOID heavy UI thread work.

## DI (Future)
SHOULD constructor: required services first; AVOID service locator; introduce interface only for multi-impl or test isolation.

## Reflection/Dynamic
AVOID unless approved; MUST justify in commit body.

## File Naming
MUST match primary type; interface suffix only if clarity improves.

## Git Hygiene
MUST no dead/commented code; SHOULD minimal diff noise; AVOID mixing style + logic in one commit.

## Deviation
Commit body line: Deviation(CodingConventions): reason

Last updated: 2025-11-05
