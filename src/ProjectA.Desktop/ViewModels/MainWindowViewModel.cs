using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Desktop.Annotations;
using ProjectA.Desktop.Services;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Desktop.ViewModels
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IRepository<Document> repository,
            ShepherdService service)
        {
            DashboardViewModel = new DashboardViewModel(repository);
            ToolbarViewModel = new ToolbarViewModel();

            service.AfterExecute += async (sender, args) =>
            {
                logger.LogInformation("Reload document by {FunctionName}", nameof(service.AfterExecute));
                await DashboardViewModel.ReloadDocuments();
            };
            ToolbarViewModel.OnRefreshDocumentsButtonClicked += async (sender, args) =>
            {
                service.DoWork();
            };
        }

        public DashboardViewModel DashboardViewModel { get; set; }
        public ToolbarViewModel ToolbarViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}