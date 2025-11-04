---
applyTo: "MockWinAppInstaller/src/**"
description: "Condensed C# coding conventions for MockWinAppInstaller (WPF MVVM) referencing official Microsoft guidance."
---

# Coding Conventions: MockWinAppInstaller

> Delta rules supplement global & base path-specific instructions. Keep implementation consistent, readable, and testable.
> Reference: [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

## Naming

- Types, public methods, properties: PascalCase.
- Private fields: camelCase; prefix with `_` only if clarity needed (avoid Hungarian).
- Interfaces: leading `I` (e.g. `IUpdateService`).
- Async methods suffix: `Async` (e.g. `RunUpdateAsync`).
- Cancellation tokens named `cancellationToken`.

## Namespaces & Files

- Use file-scoped namespace (`namespace MockWinAppInstaller;`) unless multiple types require block.
- One public class per file when practical.

## Using Directives

- Place `using` outside namespace at top.
- Order: System*, third-party, project-local (no blank lines between categories unless long list).
- Remove unused usings (enable IDE cleanup).

## Layout & Formatting

- Indent with 4 spaces; no tabs.
- Braces on new line for types/methods (Allman) OR project-wide decision: adopt default .NET style (leave consistent as existing code); do not mix.
- Keep method length focused (<50 LOC); extract helpers when exceeding.

## Fields vs Properties

- Prefer auto-properties; use backing field only for logic in getter/setter.
- Immutable config via `record` or init-only properties where feasible.

## Null Handling & Guards

- Use early guard clauses: `if (arg is null) throw new ArgumentNullException(nameof(arg));`
- Avoid nested `if` pyramids; return early.

## Async & Tasks

- Avoid `.Result` / `.Wait()` on Tasks (deadlock risk); use `await`.
- Use `ConfigureAwait(false)` only in library code; app code can omit.
- Pass cancellation token through async call chain; do not ignore.

## Exceptions

- Throw specific exceptions (ArgumentException, InvalidOperationException) vs generic.
- Do not swallow exceptions silently; log or rethrow with context.

## LINQ & Collections

- Favor clarity over chaining complexity; break long LINQ queries into local variables.
- Use `Any()` instead of `Count() > 0`.

## Strings & Interpolation

- Use string interpolation `$"Checksum: {value}"` over `string.Format` unless culture-specific formatting required.
- Localize UI/user-facing text via resources; no inline hard-coded Korean/English strings.

## Comments & Docs

- XML doc comments only for public API or complex logic; avoid redundant comments.
- Use TODO with issue reference: `// TODO(#123): refine progress algorithm`.

## ViewModel & Commands

- Command fields: `public ICommand StartUpdateCommand { get; }` (read-only).
- Keep UI thread logic minimal; heavy work offloaded to service layer.

## Dependency Injection (Future Ready)

- If DI introduced, constructor parameter order: required services first, optional config later.
- Avoid service locator pattern.

## Unsafe / Reflection

- Avoid `dynamic`/reflection unless approved (Ask Before list applies).

## File Naming

- Match primary type name (e.g. `UpdateSimulator.cs`).
- Suffix interfaces with service function only if clarity improved (`IChecksumService`).

## Git Hygiene (Code Context)

- Do not commit commented-out blocks; remove or justify.
- Keep diff noise low (avoid whitespace-only changes).

## Deviation

- If a lab requires style variance (e.g. tab indentation for teaching), annotate commit body: `Deviation(CodingConventions): reason`.

Last updated: 2025-11-05
