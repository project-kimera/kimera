using Kimera.Commands;
using Kimera.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        public DelegateCommand AddExecutableFileCommand { get; }

        public DelegateCommand AddArchiveFileCommand { get; }

        public DelegateCommand AddMultipleFileCommand { get; }

        public DelegateCommand AddFolderCommand { get; }

        public LibraryViewModel()
        {
            AddExecutableFileCommand = new DelegateCommand(AddExecutableFileDialog);
            AddArchiveFileCommand = new DelegateCommand(AddArchiveFileDialog);
            AddMultipleFileCommand = new DelegateCommand(AddMultipleFileDialog);
            AddFolderCommand = new DelegateCommand(AddFolderDialog);
        }

        private void AddExecutableFileDialog()
        {
            AddExecutableFileDialog dialog = new AddExecutableFileDialog();
            dialog.ShowDialog();
        }

        private void AddArchiveFileDialog()
        {
            AddArchiveFileDialog dialog = new AddArchiveFileDialog();
            dialog.ShowDialog();
        }

        private void AddMultipleFileDialog()
        {
            AddMultipleFileDialog dialog = new AddMultipleFileDialog();
            dialog.ShowDialog();
        }

        private void AddFolderDialog()
        {
            AddFolderDialog dialog = new AddFolderDialog();
            dialog.ShowDialog();
        }
    }
}
