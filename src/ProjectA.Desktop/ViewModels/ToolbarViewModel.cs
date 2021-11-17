#nullable enable
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ProjectA.Desktop.Annotations;
using ProjectA.Desktop.Controls;

namespace ProjectA.Desktop.ViewModels
{
    public class ToolbarViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public ToolbarViewModel()
        {
            OpenAddNewDocumentDialogCommand = new AnotherCommandImplementation(OpenAddNewDocumentDialog);
            ForceRefreshDocumentsCommand = new AnotherCommandImplementation(RefreshDocuments);
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler OnRefreshDocumentsButtonClicked;


        private void RefreshDocuments(object obj)
        {
            OnRefreshDocumentsButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OpenAddNewDocumentDialog(object obj)
        {
            DialogContent = new AddDocumentControl();
            IsAddDocumentDialogOpen = true;
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands

        public ICommand OpenAddNewDocumentDialogCommand { get; }
        public ICommand ForceRefreshDocumentsCommand { get; }

        #endregion

        #region Properties

        public object? DialogContent { get; set; }
        public bool IsAddDocumentDialogOpen { get; set; }
        public bool IsListening { get; set; } = true;

        #endregion
    }
}