# HoL Structure & githubwrite Usage Guide

## How to use githubwrite

The "githubwrite" Copilot Chat capability uses a structure/spec markdown file as contextual guidance to generate, refactor, and document code inside the current repository.

### ✅ What you CAN do

- Generate new source files matching a described directory tree.
- Insert WPF/.NET boilerplate (e.g., `App.xaml`, `MainWindow.xaml`, `.csproj`).
- Propose `.gitignore` entries for platform and tooling artifacts.
- Expand documentation sections (README encryption notes, lab guides).
- Suggest safe git workflows (fetch, branch, commit, push) as commands.
- Refactor simple logic into `Services/` or `ViewModels/` layers.
- Provide prompt examples (async operations, MVVM bindings, localization).
- Enumerate architectural layout & future extension ideas.
- Add localization placeholders and basic MVVM patterns.

### ❌ What you CANNOT (or should not expect) it to do

- Create a brand‑new remote GitHub repository automatically (manual or `gh` CLI required).
- Push commits without explicit user confirmation/tools (it only suggests commands).
- Read secrets outside the workspace or exfiltrate encrypted data.
- Decrypt git-crypt content without proper GPG key configured.
- Execute GUI apps or long‑running installers.
- Guarantee licensing/legal compliance beyond inserting provided text.
- Perform binary security scanning or vulnerability auditing.
- Purge published sensitive history without you running rewrite tools.
- Resolve complex merge conflicts without human validation.

### 📸 Copilot Chat githubwrite Screenshot

Single capture showing a structure spec used in Copilot Chat to drive repository edits:

![Copilot githubwrite panel](assets/githubwrite-in-Copilot-Chat.png)

> If the image does not appear: verify `assets/githubwrite-in-Copilot-Chat.png` exists and is committed.

---

## Repository Structure

This document describes the intended directory and file layout for the `GitHub-Copilot-HoL` repository and the `MockWinAppInstaller` WPF example project.

```text
GitHub-Copilot-HoL/
│
├── README.md                        # Repository overview and Hands-on Labs intro
├── LICENSE                          # Root license
│
└── MockWinAppInstaller/             # WPF sample application (HoL target)
    ├── README.md                    # Project-level description
    ├── LICENSE                      # Project-specific (duplicate or custom) license
    ├── MockWinAppInstaller.sln      # Visual Studio solution file
    ├── .gitignore                   # Project-local ignore rules (WPF/VS)
    │
    ├── docs/                        # Documentation and supporting materials
    │   ├── copilot-instructions.md  # GitHub Copilot usage guidance & prompt examples
    │   ├── design-spec.md           # Architectural and component design specification
    │   └── assets/                  # Images referenced by docs
    │       ├── architecture-diagram.png  # Architecture diagram
    │       └── ui-wireframe.png          # UI wireframe
    │
    └── src/                         # WPF application source code
        ├── MockWinAppInstaller.csproj    # .NET project file (WPF)
        ├── App.xaml                       # Application resource root & startup URI
        ├── App.xaml.cs                    # Application class code-behind
        ├── MainWindow.xaml                # Main window XAML UI
        ├── MainWindow.xaml.cs             # Main window code-behind
        ├── ViewModels/
        │    └── MainViewModel.cs          # Primary ViewModel (MVVM)
        ├── Services/
        │    ├── FileService.cs            # File selection & basic I/O service
        │    ├── ChecksumService.cs        # Checksum/hash computation service
        │    └── UpdateSimulator.cs        # Update workflow simulation service
        ├── Properties/
        │    ├── AssemblyInfo.cs           # Assembly metadata / ThemeInfo
        │    ├── Resources.resx            # Neutral (default) resources
        │    └── Resources.ko.resx         # Korean localized resources
        │
        ├── bin/                           # Build output (excluded by .gitignore)
        │   ├── Debug/
        │   └── Release/
        │
        └── obj/                           # Intermediate build artifacts (excluded)
```

## Overview

The `GitHub-Copilot-HoL` repository aggregates multiple Hands-on Labs (HoLs) demonstrating GitHub Copilot. The `MockWinAppInstaller` project is a focused WPF example application used to practice prompting and iterative development with Copilot.

## Key Folders

| Path | Purpose |
|------|---------|
| `MockWinAppInstaller/docs` | Developer and learning documentation |
| `MockWinAppInstaller/docs/assets` | Visual assets (architecture, wireframes) |
| `MockWinAppInstaller/src` | All application source code (MVVM + services) |
| `MockWinAppInstaller/src/ViewModels` | ViewModel classes for MVVM pattern |
| `MockWinAppInstaller/src/Services` | Service abstractions (file, checksum, simulation) |
| `MockWinAppInstaller/src/Properties` | Assembly and resource localization files |

## Suggested Future Extensions

- Add a test project: `MockWinAppInstaller.Tests/`
- Introduce logging abstraction (e.g., `ILogger` wrapper)
- Add GitHub Actions workflow: `.github/workflows/build.yml`
- Expand localization (e.g., `Resources.en.resx`)
- Capture Architecture Decision Records (ADR) in `docs/adr/`

## Copilot Prompt Ideas

| Goal | Example Prompt |
|------|----------------|
| Async checksum | "Generate an async method in ChecksumService to compute SHA256 for a selected file." |
| MVVM binding | "Refactor MainWindow code-behind into MainViewModel with ICommand bindings." |
| Progress simulation | "Create a progress-reporting update simulation using IProgress<double>." |
| Localization | "Suggest keys and values for installer UI localization in Korean." |

## Notes

- The `bin/` and `obj/` folders should remain excluded via `.gitignore`.
- Duplicate LICENSE inside `MockWinAppInstaller/` is optional—use if sub-project should stand alone.
- Assets are placeholders until real diagrams and wireframes are added.

---

## Maintenance Tips
- When adding new encrypted assets (git-crypt), append explicit lines to `.gitattributes`.
- Keep this structure file updated if new lab modules or example projects are added.
