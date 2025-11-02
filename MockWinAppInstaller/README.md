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

## 🌐 Localization
Resource files: `Properties/Resources.resx`, `Properties/Resources.ko.resx` (extend with UI strings and messages).

## ▶️ Running (Windows)
```bash
# From solution root (future .sln placement)
dotnet build MockWinAppInstaller/src/MockWinAppInstaller.csproj
# To run:
dotnet run --project MockWinAppInstaller/src/MockWinAppInstaller.csproj
```

## 🔮 Next Steps (Optional)
- Bind `MainViewModel` to `MainWindow.xaml`
- Wire `UpdateSimulator` into a progress bar
- Add checksum input + validate button
- Expand localization keys
- Add basic unit tests

---
_This README is an initial placeholder; evolve as architecture and specs grow._
