using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocAggregate.Specifications;
using ProjectA.Desktop.Annotations;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Desktop.ViewModels
{
    public class AddDocumentViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Document> _repository;


        public AddDocumentViewModel(IRepository<Document> repository)
        {
            _repository = repository;
            SubmitCommand = new AnotherCommandImplementation(Submit, CanSubmit);
            CancelCommand = new AnotherCommandImplementation(Cancel);
        }

        public int Id { get; set; } = 669486;
        public int TargetFolderId { get; set; } = 75696;

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }


        public event PropertyChangedEventHandler PropertyChanged;

        private bool CanSubmit(object arg)
        {
            var spec = new DocumentByEntityIdSpec(Id);
            var document = _repository.GetBySpecAsync(spec).GetAwaiter().GetResult();
            return document == null;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Submit(object o)
        {
            var document = new Document(Id, TargetFolderId);
            _repository.AddAsync(document);
            _repository.SaveChangesAsync();

            ViewModelLocator.MainWindowViewModel.DashboardViewModel.Documents.Add(document);
            ViewModelLocator.MainWindowViewModel.ToolbarViewModel.IsAddDocumentDialogOpen = false;
        }

        private void Cancel(object obj)
        {
            ViewModelLocator.MainWindowViewModel.ToolbarViewModel.IsAddDocumentDialogOpen = false;
        }
    }
}