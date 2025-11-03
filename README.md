# GitHub Copilot Hands-on Labs

This repository contains various Hands-on Labs (HoLs) demonstrating key GitHub Copilot features.

## Structure

- MockWinAppInstaller: Sample WPF application used in exercises.
- docs: Design, Copilot usage guidance, and assets.

## Getting Started

Navigate to `MockWinAppInstaller/` and open the solution file in Visual Studio.

## 🔐 Encrypted Customer Specification (git-crypt)
This repository uses git-crypt to transparently encrypt sensitive customer documentation (e.g. `MockWinAppInstaller/docs/spec-from-customer.md` and related asset images). The encrypted blobs appear as unreadable binary on GitHub, but authorized collaborators with a registered GPG key automatically see plaintext locally after `git-crypt unlock`.
- History was rewritten to remove prior plaintext for the spec file.
- To grant access: supply a GPG public key and run `git-crypt add-gpg-user <KEY_ID>`.
- To verify encryption: view the file on GitHub; it should NOT display readable customer content.
