using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using MockWinAppInstaller.Services;
using System.Net;
using System.Management; // WMI for USB/MTP device detection (heuristic)

// ============================================================================
// File: MainViewModel.cs
// Purpose: Central state & command logic for the Mock Installer UI.
// Summary of Implemented Features (2025-11-05):
//   * Dynamic localization (runtime culture switch via LocalizedStrings).
//   * File selection with size + CRC16 checksum display & destination verification.
//   * Simulated patch/update progress (async copy with progress bar, completion + error dialogs).
//   * Segmented IPv4 editing w/ per-octet validation + red highlight + focus restore.
//   * Registry persistence of last 10 IP addresses (HKCU\Software\MockWinAppInstaller\RecentIps).
//   * USB-B heuristic detection (removable drives + filtered WMI PnP entities) gating IP edit.
//   * Status text fully localized (idle, in-progress, complete, mismatch, errors, language failure).
// Security / Integrity Notes:
//   * No external network calls; only local file IO & registry access.
//   * Checksum mismatch halts normal completion flow and shows error.
//   * WMI detection is heuristic; not authoritative for connector type.
// Future Extensibility:
//   * Add cancellation to patches (wire CancelUpdateCommand).
//   * Replace CRC16 with SHA-256 signed manifest for production.
//   * Async caching of USB detection to reduce WMI overhead.
// ============================================================================
namespace MockWinAppInstaller.ViewModels
{
    /// <summary>
    /// ViewModel exposing commands and state for installer operations (file selection,
    /// patch simulation, IP configuration, localization, USB gating, checksums).
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // Basic status / progress
        private string _statusText = "Idle"; // will be localized on ctor
    /// <summary>Current localized status line.</summary>
    public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }

        private double _progressValue;
    /// <summary>Numeric progress (0..100) updated by simulator.</summary>
    public double ProgressValue { get => _progressValue; set { _progressValue = value; OnPropertyChanged(); OnPropertyChanged(nameof(ProgressPercent)); } }
    /// <summary>Formatted percent string for UI.</summary>
    public string ProgressPercent => $"{ProgressValue:0}%";

        // File / checksum info
        private string _appImagePath = string.Empty;
    /// <summary>Selected patch file path (read-only textbox in UI).</summary>
    public string AppImagePath { get => _appImagePath; set { _appImagePath = value; OnPropertyChanged(); } }

        private string _fileSizeDisplay = "0 Bytes";
    /// <summary>Human-readable size of selected file.</summary>
    public string FileSizeDisplay { get => _fileSizeDisplay; set { _fileSizeDisplay = value; OnPropertyChanged(); } }

    private string _checksumDisplay = string.Empty; // blank default until file selected
    /// <summary>CRC16 checksum of original file (empty until selection).</summary>
    public string ChecksumDisplay { get => _checksumDisplay; set { _checksumDisplay = value; OnPropertyChanged(); } }

    /// <summary>Flag reserved for enabling checksum logic toggle (future expansion).</summary>
    public bool IsChecksumEnabled { get => _isChecksumEnabled; set { _isChecksumEnabled = value; OnPropertyChanged(); } }
        private bool _isChecksumEnabled;
        public bool IsMemoryInitEnabled { get => _isMemoryInitEnabled; set { _isMemoryInitEnabled = value; OnPropertyChanged(); } }
        private bool _isMemoryInitEnabled;

        // Module selection
    /// <summary>Mock list of selectable modules (placeholder values).</summary>
    public ObservableCollection<string> Modules { get; } = new() { "GIPAM3000 주처리(61850)", "SampleModuleA", "SampleModuleB" };
        private string? _selectedModule;
        public string? SelectedModule { get => _selectedModule; set { _selectedModule = value; OnPropertyChanged(); } }

        private bool _isNonReplaceable;
        public bool IsNonReplaceable { get => _isNonReplaceable; set { _isNonReplaceable = value; OnPropertyChanged(); } }

        // Connection / timestamp
        private string _connectionIp = "172.30.0.33"; // placeholder
    /// <summary>Currently active IP (auto-detected initially, then user-modified).</summary>
    public string ConnectionIp { get => _connectionIp; set { _connectionIp = value; OnPropertyChanged(); } }

        private string _lastTimestamp = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
    /// <summary>Timestamp of last successful patch or file load.</summary>
    public string LastTimestamp { get => _lastTimestamp; set { _lastTimestamp = value; OnPropertyChanged(); } }

        // IP dialog editable candidate (composed from 4 octets)
        private string _newIpCandidate = string.Empty;
    /// <summary>Composed IP from four octet inputs (may be invalid until validation).</summary>
    public string NewIpCandidate { get => _newIpCandidate; private set { _newIpCandidate = value; OnPropertyChanged(); } }

    private string _newIpOctet1 = ""; public string NewIpOctet1 { get => _newIpOctet1; set { _newIpOctet1 = value; OnPropertyChanged(); RecomputeNewIpCandidate(); ValidateOctet(1); } }
    private string _newIpOctet2 = ""; public string NewIpOctet2 { get => _newIpOctet2; set { _newIpOctet2 = value; OnPropertyChanged(); RecomputeNewIpCandidate(); ValidateOctet(2); } }
    private string _newIpOctet3 = ""; public string NewIpOctet3 { get => _newIpOctet3; set { _newIpOctet3 = value; OnPropertyChanged(); RecomputeNewIpCandidate(); ValidateOctet(3); } }
    private string _newIpOctet4 = ""; public string NewIpOctet4 { get => _newIpOctet4; set { _newIpOctet4 = value; OnPropertyChanged(); RecomputeNewIpCandidate(); ValidateOctet(4); } }

        private void RecomputeNewIpCandidate()
        {
            NewIpCandidate = string.Join('.', new[] { NewIpOctet1, NewIpOctet2, NewIpOctet3, NewIpOctet4 });
        }

        private readonly ChecksumService _checksumService = new();
        private readonly Services.UpdateSimulator _updateSimulator = new();
        private bool _isUpdating;
        private string _initialIp = "0.0.0.0";
        public string InitialIp => _initialIp;
        // Octet validity flags
        private bool _isOctet1Invalid; public bool IsOctet1Invalid { get => _isOctet1Invalid; set { _isOctet1Invalid = value; OnPropertyChanged(); } }
        private bool _isOctet2Invalid; public bool IsOctet2Invalid { get => _isOctet2Invalid; set { _isOctet2Invalid = value; OnPropertyChanged(); } }
        private bool _isOctet3Invalid; public bool IsOctet3Invalid { get => _isOctet3Invalid; set { _isOctet3Invalid = value; OnPropertyChanged(); } }
        private bool _isOctet4Invalid; public bool IsOctet4Invalid { get => _isOctet4Invalid; set { _isOctet4Invalid = value; OnPropertyChanged(); } }

    // USB usage toggle
        private bool _isUsbUseChecked; public bool IsUsbUseChecked {
            get => _isUsbUseChecked; 
            set {
                if (_isUsbUseChecked == value) return;
                if (value)
                {
                    bool usbPresent = false;
                    try { usbPresent = DetectUsbBDevices(); } catch { usbPresent = false; }
                    if (!usbPresent)
                    {
                        System.Windows.MessageBox.Show(_loc?.IpDialogUsbNotConnected ?? "USB device not detected", _loc?.IpDialogTitle ?? "IP Settings", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        _isUsbUseChecked = false; // ensure unchecked
                        OnPropertyChanged();
                        OnPropertyChanged(nameof(IsIpEditingEnabled));
                        FocusFirstOctet();
                        return;
                    }
                }
                _isUsbUseChecked = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsIpEditingEnabled));
                if (!value) FocusFirstOctet();
            }
        }
    /// <summary>True when IP fields editable (USB gating not active).</summary>
    public bool IsIpEditingEnabled => !IsUsbUseChecked;

        // Commands
        public ICommand BrowseAppImageCommand { get; }
        public ICommand SelectModuleCommand { get; }
        public ICommand StartUpdateCommand { get; }
        public ICommand CancelUpdateCommand { get; }
        public ICommand OpenCommModelDialogCommand { get; }
        public ICommand OpenIpSettingsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetLanguageKoreanCommand { get; }
        public ICommand SetLanguageEnglishCommand { get; }
        public ICommand SaveIpSettingsCommand { get; }
        public ICommand CancelIpSettingsCommand { get; }
        private Services.LocalizedStrings? _loc => System.Windows.Application.Current.Resources["Loc"] as Services.LocalizedStrings;

        public MainViewModel()
        {
            BrowseAppImageCommand = new RelayCommand(async _ => await BrowseFileAsync());
            SelectModuleCommand = new RelayCommand(_ => { /* TODO: implement module selection */ });
            StartUpdateCommand = new RelayCommand(_ => StartPatch());
            CancelUpdateCommand = new RelayCommand(_ => { /* TODO */ });
            OpenCommModelDialogCommand = new RelayCommand(_ => { /* TODO: open comm model dialog */ });
            OpenIpSettingsCommand = new RelayCommand(_ => OpenIpDialog());
            ExitCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());
            SaveIpSettingsCommand = new RelayCommand(_ => SaveIpDialog());
            CancelIpSettingsCommand = new RelayCommand(_ => CloseActiveDialog());

            SelectedModule = Modules.Count > 0 ? Modules[0] : null;
            SetLanguageKoreanCommand = new RelayCommand(_ => ChangeLanguage("ko"));
            SetLanguageEnglishCommand = new RelayCommand(_ => ChangeLanguage("en"));
            if (_loc != null) _statusText = _loc.StatusIdle; OnPropertyChanged(nameof(StatusText));
            RefreshCurrentIp();
            _initialIp = ConnectionIp; // store first detected IP
        }

        private System.Windows.Window? _activeDialog;
        private void OpenIpDialog()
        {
            if (_activeDialog != null) return; // prevent multiple
            RefreshCurrentIp();
            var parts = ConnectionIp.Split('.');
            if (parts.Length == 4)
            {
                NewIpOctet1 = parts[0];
                NewIpOctet2 = parts[1];
                NewIpOctet3 = parts[2];
                NewIpOctet4 = parts[3];
            }
            IsOctet1Invalid = IsOctet2Invalid = IsOctet3Invalid = IsOctet4Invalid = false;
            var win = new MockWinAppInstaller.IpSettingsWindow
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = this
            };
            // Pre-detect USB/MTP devices using WMI; fallback to removable drive
            try { IsUsbUseChecked = DetectUsbBDevices(); }
            catch { IsUsbUseChecked = false; }
            _activeDialog = win;
            win.Closed += (_, __) => { _activeDialog = null; };
            win.ShowDialog();
        }

        private void SaveIpDialog()
        {
            // Re-run validation explicitly before save
            bool allValid = ValidateAllOctets();
            bool anyBlank = string.IsNullOrWhiteSpace(NewIpOctet1) || string.IsNullOrWhiteSpace(NewIpOctet2) || string.IsNullOrWhiteSpace(NewIpOctet3) || string.IsNullOrWhiteSpace(NewIpOctet4);
            if (anyBlank)
            {
                System.Windows.MessageBox.Show(_loc?.IpDialogErrorBlankOctet ?? "One or more IP octets are blank.", _loc?.IpDialogTitle ?? "IP Settings", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                FocusFirstInvalidOrBlankOctet();
                return;
            }
            // Validate overall IPv4 format
            if (!allValid || !IsValidIPv4(NewIpCandidate))
            {
                System.Windows.MessageBox.Show(_loc?.IpDialogErrorInvalidIp ?? _loc?.IpDialogInvalid ?? "Invalid IP address format.", _loc?.IpDialogTitle ?? "IP Settings", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                FocusFirstInvalidOrBlankOctet();
                return;
            }
            ConnectionIp = NewIpCandidate;
            StatusText = ConnectionIp == _initialIp ? (_loc?.StatusIpSame ?? "IP 동일") : (_loc?.StatusIpChanged ?? "IP 변경 감지");
            PersistRecentIp(ConnectionIp);
            CloseActiveDialog();
        }

        public void ValidateOctet(int index)
        {
            string value = index switch
            {
                1 => NewIpOctet1,
                2 => NewIpOctet2,
                3 => NewIpOctet3,
                4 => NewIpOctet4,
                _ => string.Empty
            };
            bool invalid = !IsValidOctet(value);
            switch (index)
            {
                case 1: IsOctet1Invalid = invalid; break;
                case 2: IsOctet2Invalid = invalid; break;
                case 3: IsOctet3Invalid = invalid; break;
                case 4: IsOctet4Invalid = invalid; break;
            }
        }
        private bool IsValidOctet(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (!int.TryParse(value, out int num)) return false;
            return num >= 0 && num <= 255;
        }
        public bool ValidateAllOctets()
        {
            ValidateOctet(1); ValidateOctet(2); ValidateOctet(3); ValidateOctet(4);
            return !(IsOctet1Invalid || IsOctet2Invalid || IsOctet3Invalid || IsOctet4Invalid);
        }

        private void FocusFirstInvalidOctet()
        {
            if (_activeDialog is MockWinAppInstaller.IpSettingsWindow win)
            {
                System.Windows.Controls.TextBox? focusTarget = null;
                if (IsOctet1Invalid) focusTarget = win.FindName("Octet1Box") as System.Windows.Controls.TextBox;
                else if (IsOctet2Invalid) focusTarget = win.FindName("Octet2Box") as System.Windows.Controls.TextBox;
                else if (IsOctet3Invalid) focusTarget = win.FindName("Octet3Box") as System.Windows.Controls.TextBox;
                else if (IsOctet4Invalid) focusTarget = win.FindName("Octet4Box") as System.Windows.Controls.TextBox;
                focusTarget?.Focus();
            }
        }
        private void FocusFirstInvalidOrBlankOctet()
        {
            if (_activeDialog is MockWinAppInstaller.IpSettingsWindow win)
            {
                System.Windows.Controls.TextBox? focusTarget = null;
                if (string.IsNullOrWhiteSpace(NewIpOctet1) || IsOctet1Invalid) focusTarget = win.FindName("Octet1Box") as System.Windows.Controls.TextBox;
                else if (string.IsNullOrWhiteSpace(NewIpOctet2) || IsOctet2Invalid) focusTarget = win.FindName("Octet2Box") as System.Windows.Controls.TextBox;
                else if (string.IsNullOrWhiteSpace(NewIpOctet3) || IsOctet3Invalid) focusTarget = win.FindName("Octet3Box") as System.Windows.Controls.TextBox;
                else if (string.IsNullOrWhiteSpace(NewIpOctet4) || IsOctet4Invalid) focusTarget = win.FindName("Octet4Box") as System.Windows.Controls.TextBox;
                focusTarget?.Focus();
            }
        }

        private void FocusFirstOctet()
        {
            if (_activeDialog is MockWinAppInstaller.IpSettingsWindow win)
            {
                (win.FindName("Octet1Box") as System.Windows.Controls.TextBox)?.Focus();
            }
        }

        // Heuristic detection aimed at USB-B style mass/storage devices only:
        // 1. Removable drives with drive letters.
        // 2. WMI PnP entities containing storage-ish keywords while excluding hubs, controllers, displays.
        private bool DetectUsbBDevices()
        {
            // (1) Removable drive letter present implies a USB storage mass device; treat as found.
            try
            {
                if (System.IO.DriveInfo.GetDrives().Any(d => d.DriveType == DriveType.Removable && d.IsReady))
                    return true;
            }
            catch { /* ignore */ }

            // (2) WMI PnP entity heuristic
            var includeKeywords = new[] { "Mass", "Storage", "Android", "Phone", "iPhone", "Media" }; // storage or phone
            var excludeKeywords = new[] { "Hub", "Controller", "Display", "Monitor", "Camera", "Keyboard", "Mouse" };
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name, PNPClass FROM Win32_PnPEntity WHERE Name LIKE '%USB%' OR Name LIKE '%Android%' OR Name LIKE '%Phone%' OR Name LIKE '%iPhone%'");
                foreach (var obj in searcher.Get())
                {
                    var name = obj["Name"]?.ToString();
                    if (string.IsNullOrWhiteSpace(name)) continue;
                    var upper = name.ToUpperInvariant();
                    // Exclusion first
                    if (excludeKeywords.Any(k => upper.Contains(k.ToUpperInvariant()))) continue;
                    // Must contain at least one include keyword OR explicit USB Storage phrase
                    if (includeKeywords.Any(k => upper.Contains(k.ToUpperInvariant())) || upper.Contains("USB") && upper.Contains("STORAGE"))
                        return true;
                }
            }
            catch { /* WMI unavailable, treat as not present */ }
            return false;
        }

        private void CloseActiveDialog()
        {
            _activeDialog?.Close();
            _activeDialog = null;
        }

        private bool IsValidIPv4(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;
            var parts = ip.Split('.');
            if (parts.Length != 4) return false;
            foreach (var p in parts)
            {
                if (!byte.TryParse(p, out _)) return false;
            }
            return true;
        }

        private void RefreshCurrentIp()
        {
            try
            {
                string selected = "0.0.0.0";
                var host = Dns.GetHostName();
                var entry = Dns.GetHostEntry(host);
                foreach (var addr in entry.AddressList)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(addr))
                    {
                        selected = addr.ToString();
                        break;
                    }
                }
                ConnectionIp = selected;
            }
            catch { ConnectionIp = "0.0.0.0"; }
        }

        private void PersistRecentIp(string ip)
        {
            try
            {
                const string baseKey = "Software/MockWinAppInstaller";
                using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(baseKey);
                if (key == null) return;
                var existing = (key.GetValue("RecentIps") as string) ?? string.Empty;
                var list = new System.Collections.Generic.List<string>();
                foreach (var item in existing.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!list.Contains(item)) list.Add(item);
                }
                list.Remove(ip);
                list.Insert(0, ip);
                if (list.Count > 10) list.RemoveRange(10, list.Count - 10);
                key.SetValue("RecentIps", string.Join(',', list));
            }
            catch { /* ignore registry errors */ }
        }

        private async void StartPatch()
        {
            if (_isUpdating) return;
            if (string.IsNullOrWhiteSpace(AppImagePath) || !File.Exists(AppImagePath))
            {
                StatusText = _loc?.StatusNoFileSelected ?? "파일 선택 없음";
                return;
            }
            try
            {
                _isUpdating = true;
                ProgressValue = 0;
                StatusText = _loc?.StatusPatchInProgress ?? "패치 진행 중";
                var progress = new Progress<double>(p => { ProgressValue = p; });
                string targetFolder = "C:\\TEMP";
                var dest = await _updateSimulator.CopyWithProgressAsync(AppImagePath, targetFolder, progress);
                // After copy, compute destination checksum and compare
                string destChecksum = await _checksumService.ComputeFileCrc16Async(dest);
                string sourceChecksum = ChecksumDisplay;
                if (!string.IsNullOrWhiteSpace(sourceChecksum) && !string.Equals(sourceChecksum, destChecksum, StringComparison.OrdinalIgnoreCase))
                {
                    StatusText = _loc?.StatusChecksumMismatch ?? "체크섬 불일치";
                    System.Windows.MessageBox.Show($"{_loc?.StatusChecksumMismatch ?? "Checksum mismatch"}\nSource: {sourceChecksum}\nDest: {destChecksum}", _loc?.StatusPatchError ?? "패치 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    ProgressValue = 0; // reset
                    return; // abort normal success flow
                }
                StatusText = _loc?.StatusPatchComplete ?? "패치 완료";
                LastTimestamp = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                System.Windows.MessageBox.Show(($"{_loc?.StatusPatchComplete ?? "패치 완료"}:\n{dest}"), _loc?.StatusPatchComplete ?? "패치 완료", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ProgressValue = 0;
            }
            catch (Exception ex)
            {
                StatusText = ($"{_loc?.StatusPatchError ?? "패치 오류"}: {ex.Message}");
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private async Task BrowseFileAsync()
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Title = "Select Patch",
                    Filter = "All Files (*.*)|*.*",
                    Multiselect = false
                };
                if (dlg.ShowDialog() != true) return;

                AppImagePath = dlg.FileName;
                var fi = new FileInfo(dlg.FileName);
                FileSizeDisplay = _checksumService.FormatSize(fi.Length);
                LastTimestamp = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

                ChecksumDisplay = await _checksumService.ComputeFileCrc16Async(dlg.FileName);
                StatusText = _loc?.StatusFileLoadComplete ?? "파일 로드 완료";
            }
            catch (Exception ex)
            {
                StatusText = ($"{_loc?.StatusPatchError ?? "패치 오류"}: {ex.Message}");
            }
        }

        private void ChangeLanguage(string culture)
        {
            try
            {
                var ci = new System.Globalization.CultureInfo(culture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
                System.Threading.Thread.CurrentThread.CurrentCulture = ci;
                _loc?.ChangeCulture(culture);
                StatusText = _loc?.StatusIdle ?? StatusText;
            }
            catch (Exception ex)
            {
                StatusText = ($"{_loc?.StatusLanguageChangeFailed ?? "언어 변경 실패"}: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
