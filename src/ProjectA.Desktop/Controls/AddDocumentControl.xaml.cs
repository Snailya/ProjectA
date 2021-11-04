using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ProjectA.Desktop.ViewModels;

namespace ProjectA.Desktop.Controls
{
    public partial class AddDocumentControl : UserControl
    {
        public AddDocumentControl()
        {
            InitializeComponent();
            DataContext = App.Host.Services.GetRequiredService<AddDocumentViewModel>();
        }
    }
    
}