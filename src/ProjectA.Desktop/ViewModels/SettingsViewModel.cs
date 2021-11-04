using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectA.Core.Services;
using ProjectA.Desktop.Annotations;
using ProjectA.Services;

namespace ProjectA.Desktop.ViewModels
{
    public class SettingsViewModel:INotifyPropertyChanged
    {
        private readonly ShepherdService _shepherdService;

        public SettingsViewModel(ShepherdService shepherdService)
        {
            _shepherdService = shepherdService;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}