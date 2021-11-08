using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectA.Core.Data;
using ProjectA.Desktop.Annotations;
using ProjectA.Desktop.Services;

namespace ProjectA.Desktop.ViewModels
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ILogger<MainWindowViewModel> _logger;

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger,
            IDbContextFactory<DocumentContext> dbContextFactory,
            ShepherdService service)
        {
            _logger = logger;
            DashboardViewModel = new DashboardViewModel(dbContextFactory);
            ToolbarViewModel = new ToolbarViewModel();

            service.AfterExecute += (sender, args) =>
            {
                _logger.LogInformation("Reload document by {FunctionName}", nameof(service.AfterExecute));
                DashboardViewModel.ReloadDocuments();
            };
            ToolbarViewModel.OnRefreshDocumentsButtonClicked += (sender, args) => { service.ExecuteImmediately(); };
            ToolbarViewModel.OnSynchronizeButtonToggled += (sender, b) => { service.EnableSync = b; };
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