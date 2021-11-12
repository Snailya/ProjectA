using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocAggregate.Specifications;
using ProjectA.Desktop.Annotations;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Desktop.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Document> _repository;

        public DashboardViewModel(IRepository<Document> repository)
        {
            _repository = repository;

            ReloadDocuments().GetAwaiter().GetResult();
        }

        public ObservableCollection<Document> Documents { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task ReloadDocuments()
        {
            var spec = new DocumentsWithLinkedDocSpec();
            Documents = new ObservableCollection<Document>((await _repository.ListAsync(spec)));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}