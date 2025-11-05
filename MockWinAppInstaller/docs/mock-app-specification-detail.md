# Mock App Specification Detail

Version: 0.1 (Draft)  
Status: Working draft – confirm window pixel size & future protocol extensions.

## 1. Overview

A mock Windows installer (WPF, .NET 8, MVVM) that simulates firmware distribution & validation workflows without requiring physical devices. It focuses on: connection mode switching (USB vs Network/IP), protocol selection (MODB / DNP), checksum verification, progress simulation, localization (EN/KR), and lightweight persistence using Windows Registry.

## 2. Primary Goals

- Provide stable UX surface to iterate quickly on installer flow.
- Allow protocol & model changes mid-session (blocked only during transfer state).
- Demonstrate i18n pattern with resource key parity (English first, Korean second).
- Persist user-driven selections (IP history, protocol/model) in HKCU registry.
- Minimize code-behind; all behavior in ViewModels & Services.

## 3. Non-Goals (Current Phase)

- Real device communication (USB stack, networking sockets).
- Cryptographic signature verification.
- IPv6 handling.
- Multi-device queuing / batch update.
- Advanced theming (dark mode) or accessibility audits beyond basic focus order & labels.

## 4. Application Shell / Window

| Item | Decision |
|------|----------|
| Title | `MockWinAppInstaller` (localized) |
| Target Framework | `net8.0-windows` |
| Window Size | Fixed (assumed 960x540 – confirmation pending) |
| Resize | Disabled (NoResize) |
| TopMost | Enabled (always on top) |
| Min/Max Buttons | Present (maximize disabled by fixed size) |
| DataContext Setup | In `MainWindow` constructor or App startup |

Assumption: If actual design requires larger horizontal space (e.g., for log panel), width can be revised; treat fixed size as a constraint for initial density reproduction.

## 5. High-Level Layout

1. Menu bar (Program / Settings / View / Language / Help).
2. Left panel tabs: "App Image" & "File Path Information".
3. Right panel: Protocol & Model selection, extraction code retrieval.
4. Central section: Update progress group (status text + progress bar + Start/Cancel).
5. Status bar: Connection (IP or USB), last event time, errors.

## 6. IP Settings Dialog

| Aspect | Spec |
|--------|------|
| Launch | From menu: Settings → IP Settings (single instance) |
| Initial IP | First private IPv4 (10.x / 172.16-31.x / 192.168.x) or fallback first IPv4; else `0.0.0.0` |
| Fields | 4 segment TextBoxes (0–255) auto-focus advance |
| Validation | Segment numeric & range; invalid disables OK & shows red error text |
| USB Mode | Checkbox "USB Mode" disables IP boxes, sets status bar to "USB" |
| History | LRU max 10 entries in HKCU registry JSON array; restore last user IP if leaving USB mode |
| Persistence | Key: `HKCU\Software\MockWinAppInstaller` value: `IpHistoryJson` |
| Hotkeys | Enter = OK (if valid), Esc = Cancel |
| Error UI | Inline red text; segment border highlight optional |
| IPv6 | Not supported (future TBD) |
| Status Bar | Shows `USB` while in USB mode else current IP |

## 7. Communication Model Dialog

| Aspect | Spec |
|--------|------|
| Launch | Menu: Settings → Change Communication Model |
| Protocol Options | RadioButtons: MODB / DNP (labels not localized) |
| Model Files | Example list: `MockApp_Main.exe`, `MockApp_61850.exe`, `MockApp_MODDNP.exe` (actual list can evolve) |
| Apply Conditions | Disabled if: update running, no changes, unsupported combo |
| Unsupported Rule (Example) | ModelC.bin allowed only with DNP (placeholder rule) |
| Persistence | Registry `CommConfig` JSON (Protocol + ModelFile) |
| Logging | On apply: `[MODEL] filename / Protocol=MODB or DNP` |
| During Update | Dialog opens; apply disabled & inline message shown |
| Same Values Reapply | Suppressed (no log) |
| Hotkeys | (Optional) Enter apply if enabled; Esc close (TBD) |

## 8. Update Simulation

| Item | Spec |
|------|------|
| Phases | Idle → Validating → Transferring → Completed / Failed / Canceled |
| Random Failure | 5% injection chance (network drop or checksum mismatch) |
| Progress Increment | 1–3% random every ~150 ms (configurable) |
| Cancellation | User can cancel during Transferring; state = Canceled |
| State Reset | Protocol/model change resets progress & checksum context (outside active transfer) |
| Concurrency | Single active simulation; guarded against re-entry |

## 9. Checksum Validation

| Item | Spec |
|------|------|
| Algorithm | CRC16 (hex, uppercase, zero-padded 4 chars) – confirm final |
| Display | `Checksum: FFFF` style (localized prefix) |
| User Input | Optional field for manual checksum (compared automatically if enabled) |
| Enforcement | Mismatch blocks update start if checksum validation toggled |
| Toggle | Checkbox `DoChecksum` initiates verification immediately |

## 10. Localization

| Aspect | Spec |
|--------|------|
| Languages | English (default) + Korean |
| Resource Files | `Resources.resx`, `Resources.ko.resx` |
| Key Policy | English key added first; Korean added simultaneously; no key without both values |
| Dynamic Text | Protocol labels (MODB / DNP) left raw (non-localized) |
| Culture Switch | Menu language items invoking `ChangeLanguageCommand` |

## 11. Persistence (Registry)

| Item | Path | Format |
|------|------|--------|
| IP History | HKCU\Software\MockWinAppInstaller | JSON array `IpHistoryJson` (most-recent-first) |
| Comm Config | HKCU\Software\MockWinAppInstaller | JSON `CommConfig` with Protocol & ModelFile |
| Future | (Reserved) separation keys for user preferences (simulation speed, etc.) | N/A |

## 12. Error Handling & UX

| Scenario | Response |
|----------|---------|
| Invalid IP Segment | Inline red text + disabled OK |
| Unsupported Protocol/Model | Inline red message in dialog |
| Update Failure (random) | Status text = Failed; color styling TBD (later design) |
| Checksum Mismatch | Blocks start or shows error + remains in Idle (if enabled) |
| Update In Progress (Comm Change) | Apply disabled + tooltip "Update in progress" |

## 13. Logging

| Item | Spec |
|------|------|
| Format | Simple in-memory list (timestamp + message) |
| Export | Future optional: manual export to file |
| Entries | IP change, model/protocol apply, update start/stop/failure, checksum result |
| Rotation | None (initial phase) |

## 14. Commands (Planned / Existing)

| Command | Purpose |
|---------|--------|
| SelectFileCommand | Open firmware file dialog |
| StartUpdateCommand | Begin simulation (valid state) |
| CancelUpdateCommand | Cancel active transfer |
| VerifyChecksumCommand | Force verification (if manual trigger needed) |
| FetchCodeCommand | Generate or request extraction code (initial: local random) |
| ChangeLanguageCommand | Switch culture (EN/KR) |
| OpenIpSettingsCommand | Open single-instance IP dialog |
| OpenCommModelDialogCommand | Open comm model dialog |
| ApplyIpSettingsCommand | Save IP or USB mode decision |
| ApplyModelChangeCommand | Persist protocol/model selection |

## 15. ViewModel Key Properties

| Property | Description |
|----------|-------------|
| CurrentIp / ConnectionDisplay | Network IP or "USB" label |
| IsUsbMode | Flag controlling IP input enable state |
| IpHistory | Observable collection of previous IPs |
| CurrentProtocol | Active protocol enum (MODB / DNP) |
| CurrentModelFile | Selected model file name |
| UpdateState | Enum (Idle, Validating, Transferring, Completed, Failed, Canceled) |
| Progress | 0–100 simulation progress value |
| StatusText | User-facing status progress description |
| CalculatedChecksum | Latest computed CRC16 hex |
| UserChecksumInput | Optional user-entered checksum |
| IsChecksumValid | Result flag of comparison |
| ExtractionCode | Generated retrieval code string |
| IsUpdateRunning | Boolean convenience for dialog gating |

## 16. Testing Minimum (Per Testing Instructions)

| Area | Tests |
|------|-------|
| ChecksumService | Valid vs invalid input comparison |
| UpdateSimulator | Progress increments & cancellation sets Canceled |
| MainViewModel | Command enable/disable transitions (e.g., Start vs Cancel) |
| Registry Services | Load/Save round-trip for IP history & comm config |
| Localization | Resource key presence EN/KR parity (simple automated scan) |

## 17. Extensibility / Future Items

| Future | Notes |
|--------|-------|
| IPv6 detection | Add separate toggle & validation path |
| Dark theme | Resource dictionary swap pattern |
| Protocol metadata panel | Dynamic fields per protocol (port, baud, etc.) |
| Batch device updates | Queue + parallelism (requires concurrency design) |
| Signature verification | Introduce secure hash + trust store |
| Structured logging | Replace simple list with provider abstraction |

## 18. Security Considerations

- No secrets persisted; registry entries are benign strings.
- Ask-before rule applies if introducing encryption or external dep.
- Avoid logging raw sensitive network tokens (none used currently).

## 19. Performance Considerations

- Single timer or async loop for progress; avoid multiple concurrent timers.
- UI updates throttled (~150 ms) to prevent over-render.
- Registry operations minimal (write-on-apply only).

## 20. Accessibility / Usability

- Keyboard navigation: Tab order left-to-right, top-to-bottom in dialogs.
- Enter/Esc behaviors implemented in IP dialog (apply/cancel). Comm dialog can mirror.
- Clear red inline errors; avoid intrusive modal popups for routine validation.

## 21. Assumptions & Open Points

| Item | Status |
|------|--------|
| Window pixel size (960x540) | Needs explicit confirmation or adjustment |
| Extract Code nature | Local random token (future server/API) |
| Checksum algorithm finality | CRC16 assumed; confirm if alternative required |
| Failure scenarios expansion | Currently checksum mismatch & random transfer fail |
| Tooltip completeness | Basic; refine if user research indicates confusion |

## 22. Glossary (Local Scope)

| Term | Meaning |
|------|---------|
| MODB | Abbreviated MODBUS in UI for space constraints |
| DNP | Distributed Network Protocol |
| LRU | Least Recently Used (history ordering preference) |
| CRC16 | 16-bit cyclic redundancy check |
| HKCU | HKEY_CURRENT_USER registry hive |

## 23. Compliance with Instruction Files

- Follows path-specific architectural rules (MVVM, minimal code-behind).
- Localization keys avoid fragment concatenation.
- Registry persistence falls under allowed “persistence” with explicit spec (ask-before satisfied by documenting here).
- Testing plan aligns with minimal coverage guide.

---
**Next Review Action:** Confirm window size & checksum algorithm then mark spec v1.0.
