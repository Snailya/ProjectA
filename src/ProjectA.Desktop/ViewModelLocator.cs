using Microsoft.Extensions.DependencyInjection;
using ProjectA.Desktop.ViewModels;

namespace ProjectA.Desktop
{
    public static class ViewModelLocator
    {
        public static MainWindowViewModel MainWindowViewModel => App.Host.Services.GetRequiredService<MainWindowViewModel>();
    }
}