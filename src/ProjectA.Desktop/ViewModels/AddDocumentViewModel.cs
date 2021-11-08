using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using ProjectA.Core.Models;
using ProjectA.Desktop.Annotations;
using ProjectA.Infrastructure.Data;

namespace ProjectA.Desktop.ViewModels
{
    public class AddDocumentViewModel : INotifyPropertyChanged
    {
        private readonly IDbContextFactory<DocumentContext> _dbContextFactory;

        public AddDocumentViewModel(IDbContextFactory<DocumentContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            SubmitCommand = new AnotherCommandImplementation(Submit);
            CancelCommand = new AnotherCommandImplementation(Cancel);
        }

        public int Id { get; set; } = 669486;
        public int SnapshotFolderId { get; set; } = 75696;

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Submit(object o)
        {
            var document = new Document(Id, SnapshotFolderId);
            using var context = _dbContextFactory.CreateDbContext();
            context.Documents.Add(document);
            context.SaveChanges();
            
            ViewModelLocator.MainWindowViewModel.DashboardViewModel.Documents.Add(document);
            ViewModelLocator.MainWindowViewModel.ToolbarViewModel.IsAddDocumentDialogOpen = false;
        }

        private void Cancel(object obj)
        {
            ViewModelLocator.MainWindowViewModel.ToolbarViewModel.IsAddDocumentDialogOpen = false;
        }
    }
}