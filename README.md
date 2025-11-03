# GitHub Copilot Hands-on Labs

> 🔐 Encrypted customer spec & assets: Sensitive customer specification (`MockWinAppInstaller/docs/spec-from-customer.md`) and its design images are encrypted at rest with git-crypt. Only authorized GPG key holders see plaintext locally. See the sections below for details and backup procedure.

This repository contains various Hands-on Labs (HoLs) demonstrating key GitHub Copilot features.

## Structure

- MockWinAppInstaller: Sample WPF application used in exercises.

## Getting Started

Navigate to `MockWinAppInstaller/` and open the solution file in Visual Studio.

## 🔐 Encrypted Customer Specification (git-crypt)

This repository uses git-crypt to transparently encrypt sensitive customer documentation (e.g. `MockWinAppInstaller/docs/spec-from-customer.md` and listed asset images). The encrypted blobs appear as unreadable binary on GitHub; authorized collaborators with a registered GPG key automatically see plaintext locally after `git-crypt unlock`.

### What is encrypted?

Currently these exact files (explicitly enumerated in `.gitattributes`):

- `MockWinAppInstaller/docs/spec-from-customer.md`
- 13 PNG files under `MockWinAppInstaller/docs/assets/` (hashed names)

Future new files in `docs/assets/` are NOT automatically encrypted (wildcard removed). Add them manually to `.gitattributes` to avoid accidental plaintext commits.

### Granting access

1. Contributor creates or supplies a GPG public key (e.g. `gpg --armor --export <KEY_ID>`).
2. Maintainer runs: `git-crypt add-gpg-user <KEY_ID>` and commits resulting key metadata.
3. Contributor clones and runs: `git-crypt unlock` (after key distribution).

### Verifying encryption

- On GitHub, the spec file should not display readable text.
- Fresh clone without key: opening the spec should show binary/garbled content.
- `git check-attr -a MockWinAppInstaller/docs/spec-from-customer.md` shows `filter: git-crypt`.

### GPG key backup & recovery

Protect the encryption access by backing up the GPG private key and revocation certificate.

Recommended steps (run once per key):

```bash
# 1. Identify key
gpg --list-secret-keys --keyid-format LONG

# 2. Export private key (store in an encrypted password manager / offline vault)
gpg --export-secret-keys --armor <KEY_ID> > gpg-private-backup-<KEY_ID>.asc

# 3. Export ownertrust (to preserve trust settings)
gpg --export-ownertrust > gpg-ownertrust-backup.txt

# 4. Generate revocation certificate (store separately)
gpg --gen-revoke <KEY_ID> > gpg-revoke-<KEY_ID>.asc

# 5. (Optional) Export public key again for documentation
gpg --export --armor <KEY_ID> > gpg-public-<KEY_ID>.asc
```

Validation of backup:

```bash
# In a temporary, isolated GPG home directory:
mkdir /tmp/gpg-restore-test && chmod 700 /tmp/gpg-restore-test
GNUPGHOME=/tmp/gpg-restore-test gpg --import gpg-private-backup-<KEY_ID>.asc
GNUPGHOME=/tmp/gpg-restore-test gpg --import-ownertrust < gpg-ownertrust-backup.txt
GNUPGHOME=/tmp/gpg-restore-test gpg --list-secret-keys
```

Disaster recovery outline:

1. Restore private key & ownertrust into new machine.
2. Run `git-crypt unlock` in the repository.
3. If compromise suspected, publish revocation certificate and rotate to a new key (add new key via `git-crypt add-gpg-user`).

Backup distribution note: gpg-backup sent to Andrew's multiple email accounts with the name of "git-crypt backup - gig on Nov. 3 2025".

Security reminders:

- Never commit the exported private key or revocation certificate.
- Store backups offline or in a vault with MFA.
- Rotate keys periodically if mandated by policy.

### Adding a new encrypted asset

Append a new line to `.gitattributes` with `filter=git-crypt diff=git-crypt` for the file, then re-stage the file and commit. Example:

```bash
echo "MockWinAppInstaller/docs/assets/new-diagram.png filter=git-crypt diff=git-crypt" >> .gitattributes
git add .gitattributes MockWinAppInstaller/docs/assets/new-diagram.png
git commit -m "chore: encrypt new diagram"
```

---

## License

Licensed under the MIT License. See the `LICENSE` file for full text.

<!-- markdownlint-disable MD033 -->
<p align="center">
  <a href="https://github.com/musma21/GitHub-Copilot-HoL" title="Visitor count">
    <img src="https://visitor-badge.laobi.icu/badge?page_id=musma21.GitHub-Copilot-HoL&left_color=gray&right_color=blue&left_text=visits" alt="visits" />
  </a>
  <a href="https://github.com/musma21/GitHub-Copilot-HoL/commits/main" title="Last commit">
    <img src="https://img.shields.io/github/last-commit/musma21/GitHub-Copilot-HoL?style=flat-square" alt="last commit" />
  </a>
  <a href="./LICENSE" title="License: MIT">
    <img src="https://img.shields.io/github/license/musma21/GitHub-Copilot-HoL?style=flat-square" alt="license" />
  </a>
</p>
<!-- markdownlint-enable MD033 -->
