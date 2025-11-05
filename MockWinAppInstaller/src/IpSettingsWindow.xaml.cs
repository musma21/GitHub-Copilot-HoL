// ============================================================================
// File: IpSettingsWindow.xaml.cs
// Purpose: Code-behind for IP settings dialog. Mostly passive; logic resides
//          in MainViewModel. Provides focus points (named TextBoxes) resolved
//          via FindName in the view model.
// Notes:
//   * Keep minimal to respect MVVM separation; no heavy logic here.
// ============================================================================
using System.Windows;
using System.Windows.Controls;

namespace MockWinAppInstaller
{
    /// <summary>
    /// Interaction logic for segmented IP configuration dialog.
    /// </summary>
    public partial class IpSettingsWindow : Window
    {
        public IpSettingsWindow()
        {
            InitializeComponent();
        }

    }
}