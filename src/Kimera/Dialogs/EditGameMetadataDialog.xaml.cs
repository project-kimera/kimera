using Kimera.Data.Entities;
using Kimera.ViewModels;
using System.Windows;

namespace Kimera.Dialogs
{
    /// <summary>
    /// EditGameMetadataDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditGameMetadataDialog : Window
    {
        public EditGameMetadataDialog(GameMetadata metadata)
        {
            InitializeComponent();
            this.DataContext = new EditGameMetadataViewModel(metadata);
        }
    }
}
