using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace YoutubeDownloaderClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            if (URL_Input.Text.Trim().Length != 0)
            {
                Message.Text = "Loading...";
                await HttpRequest.getSong(URL_Input.Text);
                Message.Text = "Done";
            }
        }
    }
}
