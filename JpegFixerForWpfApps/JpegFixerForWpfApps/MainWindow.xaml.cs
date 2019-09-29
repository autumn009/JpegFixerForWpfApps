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
using System.IO;
using System.Windows.Threading;

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
            Directory.CreateDirectory(TextBoxBackupDir.Text);

            var fullpaths = Directory.GetFiles(TextBoxRootDir.Text, "*.jpg", SearchOption.AllDirectories);
            int skip = 0;
            int done = 0;
            foreach (var fullpath in fullpaths)
            {
                var image = MyImageUtil.CreateImage(fullpath);
                if (image == null)
                {
                    skip++;
                    updateStatus(skip, done, "working...");
                    if ((skip % 10) == 1) DoEvents();
                }
                else
                {
                    DoOneFile(fullpath, image);
                    done++;
                    updateStatus(skip, done, "working...");
                    DoEvents();
                }
            }
            updateStatus(skip, done, "done!");
            Console.Beep();
        }

        private void DoOneFile(string fullpath, BitmapImage image)
        {
            // backup
            int count = 0;
            string dst;
            for (; ; )
            {
                dst = System.IO.Path.Combine(TextBoxBackupDir.Text, System.IO.Path.GetFileNameWithoutExtension(fullpath) + " " + count.ToString() + ".jpg");
                if (!File.Exists(dst)) break;
                count++;
            }
            addLog($"{fullpath}=>{dst} copying");
            DoEvents();
            File.Copy(fullpath, dst);
            addLog($"{fullpath} saving");
            saveJpeg(fullpath, image);
        }

        private void saveJpeg(string fullpath, BitmapImage image)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(fullpath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }


        private void addLog(string v)
        {
            TextBoxLog.Text += v + "\r\n";
        }

        private void updateStatus(int skip, int done, string additional)
        {
            TextBlockStatus.Text = $"Skip:{skip} Done:{done} {additional}";
        }

        private void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
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
