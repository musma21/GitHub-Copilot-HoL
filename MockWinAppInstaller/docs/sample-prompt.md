# Copilot Prompt Library

## Refactoring
Rewrite #file:src/components/Button.tsx to remove duplicated color logic and use a theming hook.
Refactor #folder:src/services for better error handling; propose incremental diffs not big rewrites.

## Testing
Generate unit tests for #file:src/utils/date.ts focusing on leap-year edge cases and invalid input.
Add integration tests for #folder:src/auth emphasizing token expiry and concurrency.

## Documentation
Create a high-level README section summarizing architecture from #folder:src/core and highlight extension points.
Document public exports found in #file:src/index.ts including usage examples.

## Performance
Analyze #folder:src/api for N+1 database query patterns; suggest targeted fixes with minimal interface changes.

## Security
Perform a security pass on #folder:src/auth for injection risks; list findings before proposing code changes.

## Migration
Rewrite #file:src/legacy/configLoader.js into TypeScript preserving behavior; highlight any implicit type assumptions.

## Style Enforcement
Ensure #file:src/components/Modal.tsx conforms to our accessibility guidelines (focus management, ARIA roles).

(Select a line and trigger Copilot Chat to use it.)