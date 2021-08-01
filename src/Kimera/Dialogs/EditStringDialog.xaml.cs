using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kimera.Dialogs
{
    /// <summary>
    /// EditStringDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditStringDialog : Window
    {
        public string Caption
        {
            set
            {
                CaptionTextBlock.Text = value;
            }
        }

        public string Text
        {
            get
            {
                return ContentTextBox.Text;
            }
            set
            {
                ContentTextBox.Text = value;
            }
        }

        public EditStringDialog()
        {
            InitializeComponent();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
