using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MockWinAppInstaller.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _status = "Ready";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
