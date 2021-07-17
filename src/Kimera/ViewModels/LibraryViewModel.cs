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
        public DelegateCommand ShowAddExecutableFileCommand { get; }

        public DelegateCommand ShowAddArchiveFileCommand { get; }

        public DelegateCommand ShowAddMultipleFileCommand { get; }

        public DelegateCommand ShowAddFolderCommand { get; }

        public LibraryViewModel()
        {
            ShowAddExecutableFileCommand = new DelegateCommand(ShowAddExecutableFileDialog);
            ShowAddArchiveFileCommand = new DelegateCommand(ShowAddArchiveFileDialog);
            ShowAddMultipleFileCommand = new DelegateCommand(ShowAddMultipleFileDialog);
            ShowAddFolderCommand = new DelegateCommand(ShowAddFolderDialog);
        }

        private void ShowAddExecutableFileDialog()
        {
            FixedDialogWindow dialog = new FixedDialogWindow(new AddExecutableFileDialog());
            dialog.Title = "실행 파일 추가";
            dialog.Width = 600;
            dialog.Height = 400;
            dialog.ShowDialog();
        }

        private void ShowAddArchiveFileDialog()
        {
            FixedDialogWindow dialog = new FixedDialogWindow(new AddArchiveFileDialog());
            dialog.Title = "압축 파일 추가";
            dialog.Width = 600;
            dialog.Height = 400;
            dialog.ShowDialog();
        }

        private void ShowAddMultipleFileDialog()
        {
            FixedDialogWindow dialog = new FixedDialogWindow(new AddMultipleFileDialog());
            dialog.Title = "여러 파일 추가";
            dialog.Width = 600;
            dialog.Height = 400;
            dialog.ShowDialog();
        }

        private void ShowAddFolderDialog()
        {
            FixedDialogWindow dialog = new FixedDialogWindow(new AddFolderDialog());
            dialog.Title = "폴더 추가";
            dialog.Width = 600;
            dialog.Height = 400;
            dialog.ShowDialog();
        }
    }
}
