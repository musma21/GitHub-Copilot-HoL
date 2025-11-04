# MockWinAppInstaller

## 📦 Included Project
### 🔹 MockWinAppInstaller (C# / WPF)
A Proof-of-Concept (PoC) Windows app that simulates a firmware update process using:
- USB or Network connection
- File checksum validation
- Simulated update progress
- DNP / MODBUS protocol selection
- Multi-language UI (English / Korean)
- Localized logs and completion messages

All specifications and Copilot guidance are stored under  
[`MockWinAppInstaller/docs/copilot-instructions.md`](./docs/copilot-instructions.md).
[`MockWinAppInstaller/docs/spec-from-customer.md`](./docs/spec-from-customer.md).
> ⚠️ spec-from-customer.md is encypted by git-crypt

## 🚀 Goal
Provide a lightweight, scriptable mock environment to iterate on installer UX, localization, and protocol workflow without requiring real devices.

## 🗂 Directory Structure (src excerpt)
```
src/
  App.xaml
  MainWindow.xaml
  ViewModels/
  Services/
  Properties/
```

## 🛠 Tech Stack
- .NET 8
- WPF

## 📁 Key Folders

| Path | Purpose |
|------|---------|
| `MockWinAppInstaller/docs` | Developer and learning documentation |
| `MockWinAppInstaller/docs/assets` | Visual assets (architecture, wireframes) |
| `MockWinAppInstaller/src` | All application source code (MVVM + services) |
| `MockWinAppInstaller/src/ViewModels` | ViewModel classes for MVVM pattern |
| `MockWinAppInstaller/src/Services` | Service abstractions (file, checksum, simulation) |
| `MockWinAppInstaller/src/Properties` | Assembly and resource localization files |

## 🧩 Instruction Modules

### Core Instructions (Minimal Ingestion)

| Scope | File | Purpose |
|-------|------|---------|
| Global | `.github/copilot-instructions.md` | Repository-wide baseline rules |
| Path Base | `.github/instructions/MockWinAppInstaller.instructions.md` | Core delta for installer project |
| Path Testing | `.github/instructions/MockWinAppInstaller.testing.instructions.md` | Minimal test coverage guidance |
| Path Pitfalls | `.github/instructions/MockWinAppInstaller.pitfalls.instructions.md` | Common mistakes & mitigations |
| Agents | `AGENTS.md` | Role/style tags (git-mini, term-mini, code-mini, arch-pro) |

### Commentary Files (Onboarding & Extended Rationale)

For new team members or detailed context, see:

- **Global guidance**: `MockWinAppInstaller/docs/copilot-instructions-commentary.md`
- **Path-specific patterns**: `MockWinAppInstaller/docs/path-specific-instructions-commentary.md`
- **Agent profiles & escalation**: `MockWinAppInstaller/docs/AGENTS-commentary.md`

These commentary files expand on rules, provide examples, and explain trade-offs not included in the minimal instruction files.

## 🌐 Localization

Resource files: `Properties/Resources.resx`, `Properties/Resources.ko.resx` (extend with UI strings and messages).

## ▶️ Running (Windows)

```bash
# From solution root (future .sln placement)
dotnet build MockWinAppInstaller/src/MockWinAppInstaller.csproj
# To run:
dotnet run --project MockWinAppInstaller/src/MockWinAppInstaller.csproj
```

## 🪟 Windows Targeting & Cross-Platform Build

This project targets **`net8.0-windows`** with WPF. Key considerations:

- **Build on macOS/Linux:** The project sets `<EnableWindowsTargeting>true` in `MockWinAppInstaller.csproj`, allowing restore/build on non-Windows hosts. You can compile, but you *cannot* run the WPF UI outside Windows.
- **Run on Windows 10/11:** Execution (UI launch) requires a Windows environment. Use a local Windows machine, VM (e.g. Hyper-V, Parallels), or GitHub Actions Windows runner for automated validation.
- **Optional OS annotation:** You may add `[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows10.0.22000")]` to indicate Windows 11 as the primary target.
- **Publish (Single File example):**

  ```bash
  dotnet publish MockWinAppInstaller/src/MockWinAppInstaller.csproj \
    -c Release \
    -r win-x64 \
    --self-contained true \
    /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
  ```

- **Future multi-RID support:** Add `<RuntimeIdentifiers>win-x64;win-arm64</RuntimeIdentifiers>` for broader platform packaging.

If you split logic into a cross-platform class library (e.g. `MockWinAppInstaller.Core`), non-Windows hosts can unit test business logic without the WPF UI.

## 🔮 Next Steps (Optional)

- Bind `MainViewModel` to `MainWindow.xaml`
- Wire `UpdateSimulator` into a progress bar
- Add checksum input + validate button
- Expand localization keys
- Add basic unit tests

---
*This README is an initial placeholder; evolve as architecture and specs grow.*
