using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ProjectA.Core.Data;
using ProjectA.Core.Models;
using ProjectA.Desktop.Annotations;

namespace ProjectA.Desktop.ViewModels
{
    public class DashboardViewModel:INotifyPropertyChanged
    {
        private readonly IDbContextFactory<DocumentContext> _dbContextFactory;

        public DashboardViewModel(IDbContextFactory<DocumentContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;

            ReloadDocuments();
        }

        public ObservableCollection<Document> Documents { get; set; }

        public void ReloadDocuments()
        {
            var context = _dbContextFactory.CreateDbContext();
            Documents = new ObservableCollection<Document>(context.Documents.Include(x => x.Snapshot).ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}