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
                await CreatePythonConnection("python", URL_Input.Text);
                Message.Text = "Done";
            }
        }

        private async Task CreatePythonConnection(string pythonPath, string url)
        {
            // Create Process Info
            var psi = new ProcessStartInfo();
            psi.FileName = pythonPath;

            // Provide script and arguments
            var script = "App.py";
            var playlistUrl = url;

            psi.Arguments = $"\"{script}\" \"{playlistUrl}\"";

            // Process configuration
            psi.UseShellExecute = true;
            psi.CreateNoWindow = false;

            var process = Process.Start(psi);
            await process.WaitForExitAsync();
        }
    }
}
