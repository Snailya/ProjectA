#nullable enable
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ProjectA.Desktop.Annotations;
using ProjectA.Desktop.Controls;
using ProjectA.Services;

namespace ProjectA.Desktop.ViewModels
{
    public class ToolbarViewModel : INotifyPropertyChanged
    {
        public ToolbarViewModel( )
        {
            OpenAddNewDocumentDialogCommand = new AnotherCommandImplementation(OpenAddNewDocumentDialog);
            OpenSettingsDialogCommand = new AnotherCommandImplementation(OpenSettingsDialog);
            ForceRefreshDocumentsCommand = new AnotherCommandImplementation(ForceRefreshDocuments);
        }


        public ICommand OpenAddNewDocumentDialogCommand { get; }
        public ICommand ForceRefreshDocumentsCommand { get; }
        public ICommand OpenSettingsDialogCommand { get; }
        public object? DialogContent { get; set; }
        public bool IsAddDocumentDialogOpen { get; set; }
        public bool IsSettingsDialogOpen { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OpenSettingsDialog(object obj)
        {
            DialogContent = new SettingsControl();
            IsSettingsDialogOpen = true;        
        }

        private void ForceRefreshDocuments(object obj)
        {
            ViewModelLocator.MainWindowViewModel.SynchronizeDataFromEDoc();
            ViewModelLocator.MainWindowViewModel.DashboardViewModel.ReloadDocuments();
        }

        private void OpenAddNewDocumentDialog(object obj)
        {
            DialogContent = new AddDocumentControl();
            IsAddDocumentDialogOpen = true;
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}