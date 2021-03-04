using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YoutubeDownloaderClient
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    public partial class Download : Window
    {
        public Download()
        {
            InitializeComponent();
        }

        private void home_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow home = new MainWindow();
            this.Close();
            home.Show();
        }

        private async void download_btn_Click(object sender, RoutedEventArgs e)
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
