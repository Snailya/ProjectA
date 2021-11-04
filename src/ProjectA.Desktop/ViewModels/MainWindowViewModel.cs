using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectA.Core.Services;
using ProjectA.Desktop.Annotations;
using ProjectA.Services;

namespace ProjectA.Desktop.ViewModels
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ShepherdService _shepherdService;

        public MainWindowViewModel(DashboardViewModel dashboardViewModel, ToolbarViewModel toolbarViewModel, ShepherdService shepherdService)
        {
            _shepherdService = shepherdService;
            DashboardViewModel = dashboardViewModel;
            ToolbarViewModel = toolbarViewModel;

            DashboardViewModel.ReloadDocuments();
        }

        public DashboardViewModel DashboardViewModel { get; set; }
        public ToolbarViewModel ToolbarViewModel { get; set; }


        public void SynchronizeDataFromEDoc()
        {
            _shepherdService.SyncDocVersionsFromEDocAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}