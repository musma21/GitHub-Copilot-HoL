# Coding Conventions Commentary (Extended)

This file is NOT ingested by the model; it's for human onboarding and instructional depth. The minimal conventions file keeps token cost low; this expands rationale, examples, and edge cases.

## Goals

- Reinforce testability (pure logic > UI coupling).
- Prevent deadlocks (async discipline).
- Ensure localization parity (EN baseline, KO follow-up).
- Keep diffs small and intention-revealing.

## INotifyPropertyChanged Helper Pattern

Use a single helper to reduce boilerplate and centralize change notification:

```csharp
protected bool SetProperty<T>(ref T field, T value, string propertyName, Action? onChanged = null)
{
    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    field = value;
    onChanged?.Invoke();
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    return true;
}
```

Edge Cases:

- Large immutable objects: prefer replacing whole object (diff cost vs mutation clarity).
- Collections: raising PropertyChanged("Items") after bulk changes prevents excessive UI refresh.

## Async + Cancellation Flow

Never ignore a CancellationToken; propagate from UI command → ViewModel → Service.

```csharp
public async Task RunUpdateAsync(CancellationToken cancellationToken)
{
    for (int i = 0; i < _steps; i++)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _delayStrategy.WaitAsync(cancellationToken); // abstracted Task.Delay
        _progress = i + 1;
        OnProgressChanged(_progress);
    }
}
```

Guidelines:

- Abstract delay logic for deterministic tests.
- Use ThrowIfCancellationRequested early in loops (fail fast).
- Prefer returning a result object or enum for terminal state (Completed, Cancelled, Failed).

## Fire-and-Forget Safety

If background operation intentionally detached, document and log exceptions:

```csharp
_ = Task.Run(async () => {
    try { await _telemetry.FlushAsync(); }
    catch (Exception ex) { _log.Warn(ex, "Telemetry flush failed"); }
});
```

## Localization Keys

Always add English first, then Korean translation maintaining identical keys.

Example:

```xml
<!-- Resources.resx (EN) -->
<data name="Update_Start" xml:space="preserve"><value>Start Update</value></data>
<data name="Update_Cancel" xml:space="preserve"><value>Cancel</value></data>
```

```xml
<!-- Resources.ko.resx (KO) -->
<data name="Update_Start" xml:space="preserve"><value>업데이트 시작</value></data>
<data name="Update_Cancel" xml:space="preserve"><value>취소</value></data>
```

Avoid fragment concatenation ("Start" + " Update"). Provide whole phrase for context to translators.

## LINQ Readability

Break long chains:

```csharp
var candidates = items
    .Where(i => i.Enabled)
    .OrderByDescending(i => i.Priority)
    .ToList();
```

Then process in imperative loop if complex branching required. Avoid multiple enumerations by materializing once (`ToList()` or `ToArray()`).

## Exception Wrapping

Wrap with context rather than generic rethrow:

```csharp
try { await _checksum.VerifyAsync(path, cancellationToken); }
catch (IOException ex) { throw new UpdatePreparationException($"I/O during checksum: {path}", ex); }
```

Avoid swallowing; if truly ignorable, comment why.

## Progress Simulation Strategy

- Use injected strategy (interface) for delay and step count.
- Report progress as integer percentage or stepped units; keep it monotonic.
- Surface cancellation distinctly (do not treat Cancelled as Failed).

## Git Commit Hygiene Examples

Good: `feat(updates): add cancellation to progress simulator`
Bad: `update stuff` (ambiguous, poor traceability)
Deviation: `Deviation(CodingConventions): teaching tabs for lab 07` (documented rationale)

## Testing Edge Cases

- Cancellation mid-loop → progress does not advance after token request.
- Zero-step configuration → completes immediately (assert final state).
- Large step count with small delay → ensure no UI freeze (consider performance measurement later).

## Supply Chain Safeguard

Before adding a NuGet dependency, verify:

- Active maintenance (recent commits/releases).
- License compatibility (no viral copyleft for this educational template).
- Minimal transitive dependency graph.

## Common Pitfalls & Avoidance

| Pitfall | Risk | Mitigation |
|---------|------|-----------|
| Blocking `.Result` | UI deadlock | Always async/await end-to-end |
| Resource key drift | Localization inconsistency | Parity test or script (future) |
| Silent catch | Hidden failure | Log + contextual wrap |
| Complex getters | Hidden logic coupling | Move to method or cached field |
| Hard-coded delay | Non-deterministic tests | Inject delay strategy |


## Future Enhancements (Optional)

- Resource parity verifier script.
- Progress result object with diagnostics (duration, steps completed).
- Analyzer rule for forbidden `.Result` usage.

Last updated: 2025-11-05
