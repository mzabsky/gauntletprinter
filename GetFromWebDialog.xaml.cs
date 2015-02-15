using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GauntletPrinter
{
    /// <summary>
    /// Interaction logic for GetFromWebDialog.xaml
    /// </summary>
    public partial class GetFromWebDialog : Window
    {
        public string DeckString { get; set; }

        public GetFromWebDialog()
        {
            InitializeComponent();
        }

        private async void Download_OnClick(object sender, RoutedEventArgs e)
        {
            // co se sideboardy

            Uri uri = new Uri(this.deckUrl.Text);
            if (uri.Host == "www.mtggoldfish.com")
            {
                var match = Regex.Match(uri.LocalPath, "/deck/([0-9]+)");
                if (match.Length == 0) { MessageBox.Show("Incorrect deck URL."); }

                var id = match.Groups[1].Captures[0].ToString();

                var client = new WebClient();
                var requestUri = new Uri("http://" + uri.Host + "/deck/download/" + id + "/");
                string result = await client.DownloadStringTaskAsync(requestUri);
            }
            else
            {
                MessageBox.Show("Unsupported website.");
            }
        }
    }
}
