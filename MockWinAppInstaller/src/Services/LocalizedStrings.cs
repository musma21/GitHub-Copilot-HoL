using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MockWinAppInstaller.Services
{
    /// <summary>
    /// Dynamic resource accessor binding .resx keys into WPF via PropertyChanged.
    /// Supports runtime culture switch (ChangeCulture) triggering a mass refresh.
    /// </summary>
    public class LocalizedStrings : INotifyPropertyChanged
    {
        private readonly ResourceManager _rm = new ResourceManager("MockWinAppInstaller.Properties.Resources", Assembly.GetExecutingAssembly());

        private CultureInfo _culture = CultureInfo.CurrentUICulture;
        public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// Raise PropertyChanged for all exposed localization keys.
    /// </summary>
    private void RaiseAll()
        {
            OnPropertyChanged(nameof(MenuProgram));
            OnPropertyChanged(nameof(MenuOpenAppImage));
            OnPropertyChanged(nameof(MenuExit));
            OnPropertyChanged(nameof(MenuSettings));
            OnPropertyChanged(nameof(MenuCommModelSelect));
            OnPropertyChanged(nameof(MenuIpSettings));
            OnPropertyChanged(nameof(MenuLanguage));
            OnPropertyChanged(nameof(LanguageKorean));
            OnPropertyChanged(nameof(LanguageEnglish));
            OnPropertyChanged(nameof(LabelAppImage));
            OnPropertyChanged(nameof(LabelFileSize));
            OnPropertyChanged(nameof(LabelChecksum));
            OnPropertyChanged(nameof(LabelMemoryInit));
            OnPropertyChanged(nameof(LabelChecksumToggle));
            OnPropertyChanged(nameof(ButtonSelectApplication));
            OnPropertyChanged(nameof(ButtonUpdateProgram));
            OnPropertyChanged(nameof(LabelConnectionInfo));
            OnPropertyChanged(nameof(LabelIP));
            OnPropertyChanged(nameof(LabelStatus));
            OnPropertyChanged(nameof(LabelModuleSelect));
            OnPropertyChanged(nameof(StatusIdle));
            OnPropertyChanged(nameof(StatusIpSame));
            OnPropertyChanged(nameof(StatusIpChanged));
            OnPropertyChanged(nameof(StatusNoFileSelected));
            OnPropertyChanged(nameof(StatusPatchInProgress));
            OnPropertyChanged(nameof(StatusPatchComplete));
            OnPropertyChanged(nameof(StatusPatchError));
            OnPropertyChanged(nameof(StatusChecksumMismatch));
            OnPropertyChanged(nameof(StatusFileLoadComplete));
            OnPropertyChanged(nameof(StatusLanguageChangeFailed));
            OnPropertyChanged(nameof(AppTitle));
            OnPropertyChanged(nameof(IpDialogTitle));
            OnPropertyChanged(nameof(IpDialogCurrentIp));
            OnPropertyChanged(nameof(IpDialogNewIp));
            OnPropertyChanged(nameof(IpDialogSave));
            OnPropertyChanged(nameof(IpDialogCancel));
            OnPropertyChanged(nameof(IpDialogInvalid));
            OnPropertyChanged(nameof(IpDialogUsbUse));
            OnPropertyChanged(nameof(IpDialogUsbNotConnected));
        }
        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    /// <summary>
    /// Retrieve a resource string for the active culture; fallback to key.
    /// </summary>
    private string Get(string key) => _rm.GetString(key, _culture) ?? key;

        public string MenuProgram => Get("Menu_Program");
        public string MenuOpenAppImage => Get("Menu_OpenAppImage");
        public string MenuExit => Get("Menu_Exit");
        public string MenuSettings => Get("Menu_Settings");
        public string MenuCommModelSelect => Get("Menu_CommModelSelect");
        public string MenuIpSettings => Get("Menu_IpSettings");
        public string MenuLanguage => Get("Menu_Language");
        public string LanguageKorean => Get("Language_Korean");
        public string LanguageEnglish => Get("Language_English");
        public string LabelAppImage => Get("Label_AppImage");
        public string LabelFileSize => Get("Label_FileSize");
        public string LabelChecksum => Get("Label_Checksum");
        public string LabelMemoryInit => Get("Label_MemoryInit");
        public string LabelChecksumToggle => Get("Label_ChecksumToggle");
        public string ButtonSelectApplication => Get("Button_SelectApplication");
        public string ButtonUpdateProgram => Get("Button_UpdateProgram");
        public string LabelConnectionInfo => Get("Label_ConnectionInfo");
        public string LabelIP => Get("Label_IP");
        public string LabelStatus => Get("Label_Status");
        public string LabelModuleSelect => Get("Label_ModuleSelect");
        public string StatusIdle => Get("Status_Idle");
        public string StatusIpSame => Get("Status_IpSame");
        public string StatusIpChanged => Get("Status_IpChanged");
        public string StatusNoFileSelected => Get("Status_NoFileSelected");
        public string StatusPatchInProgress => Get("Status_PatchInProgress");
        public string StatusPatchComplete => Get("Status_PatchComplete");
        public string StatusPatchError => Get("Status_PatchError");
    public string StatusChecksumMismatch => Get("Status_ChecksumMismatch");
        public string StatusFileLoadComplete => Get("Status_FileLoadComplete");
        public string StatusLanguageChangeFailed => Get("Status_LanguageChangeFailed");
    public string AppTitle => Get("App_Title");
    public string IpDialogTitle => Get("IpDialog_Title");
    public string IpDialogCurrentIp => Get("IpDialog_CurrentIp");
    public string IpDialogNewIp => Get("IpDialog_NewIp");
    public string IpDialogSave => Get("IpDialog_Save");
    public string IpDialogCancel => Get("IpDialog_Cancel");
    public string IpDialogInvalid => Get("IpDialog_Invalid");
    public string IpDialogUsbUse => Get("IpDialog_UsbUse");
    public string IpDialogUsbNotConnected => Get("IpDialog_UsbNotConnected");

        /// <summary>
        /// Switch active culture and notify all bindings.
        /// </summary>
        public void ChangeCulture(string cultureCode)
        {
            _culture = new CultureInfo(cultureCode);
            RaiseAll();
        }
    }
}
