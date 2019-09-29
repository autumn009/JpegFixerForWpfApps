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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JpegFixerForWpfApps
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Progress.IsIndeterminate = true;
                ButtonStart.IsEnabled = false;
                DoConversionBody();
            }
            finally
            {
                Progress.IsIndeterminate = false;
                ButtonStart.IsEnabled = true;
            }
        }

        private void DoConversionBody()
        {




        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxRootDir.Text = Properties.Settings.Default.RootDir??"";
            TextBoxBackupDir.Text = Properties.Settings.Default.BackupDir ?? "";
            Progress.IsIndeterminate = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.RootDir = TextBoxRootDir.Text;
            Properties.Settings.Default.BackupDir = TextBoxBackupDir.Text;
            Properties.Settings.Default.Save();
        }
    }
}
