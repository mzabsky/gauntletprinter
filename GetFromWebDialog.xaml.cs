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
        public string SideboardString { get; set; }

        private bool isClosed = false;

        public GetFromWebDialog()
        {
            InitializeComponent();
        }

        private void ParseGoldfishDeck(string result)
        {
            result = Regex.Replace(result, "^//.*$", "", RegexOptions.Multiline);

            var sideboardSeparatorPosition = Math.Max(
                    result.LastIndexOf("\n\r", System.StringComparison.Ordinal),
                    result.LastIndexOf("\n\n", System.StringComparison.Ordinal));

            this.DeckString = result.Substring(0, sideboardSeparatorPosition).Trim();
            if (sideboardSeparatorPosition != -1)
            {
                this.SideboardString = result.Substring(sideboardSeparatorPosition + 1).Trim();
            }
        }

        private void ParseTop8Deck(string result)
        {
            this.DeckString = "";
            this.SideboardString = "";

            var deckMatches = Regex.Matches(result, @"^\s+(?<count>\d+) \[[A-Z0-9]{3}\] (?<card>.+)$", RegexOptions.Multiline);

            for (int i = 0; i < deckMatches.Count; i++)
            {
                var name = deckMatches[i].Groups["card"].Captures[0].ToString().Trim();
                var count = deckMatches[i].Groups["count"].Captures[0].ToString().Trim();

                this.DeckString += count + " " + name + Environment.NewLine;
            }
            
            var sideboardMatches = Regex.Matches(result, @"^SB:\s+(?<count>\d+) \[[A-Z0-9]{3}\] (?<card>.+)$", RegexOptions.Multiline);

            for (int i = 0; i < sideboardMatches.Count; i++)
            {
                var name = sideboardMatches[i].Groups["card"].Captures[0].ToString().Trim();
                var count = sideboardMatches[i].Groups["count"].Captures[0].ToString().Trim();

                this.SideboardString += count + " " + name + Environment.NewLine;
            }
        }

        private void ParseTappedOutDeck(string result)
        {
            var halves = result.Split(new [] {"Sideboard:"}, StringSplitOptions.None);

            this.DeckString = halves[0].Trim().Replace("/", "//");
            this.SideboardString = halves.Length > 0 ? halves[1].Trim().Replace("/", "//") : "";
        }

        private async void Download_OnClick(object sender, RoutedEventArgs e)
        {
            var client = new WebClient();

            await Task.Delay(TimeSpan.FromMilliseconds(1));

            // co se sideboardy
            this.download.IsEnabled = false;

            try
            {
                Uri uri = new Uri(this.deckUrl.Text);
                if (uri.Host == "www.mtggoldfish.com")
                {
                    var match = Regex.Match(uri.LocalPath, "/deck/([0-9]+)");
                    if (match.Length > 0)
                    {
                        var id = match.Groups[1].Captures[0].ToString();

                        var requestUri = new Uri("http://" + uri.Host + "/deck/download/" + id + "/");
                        string result = await client.DownloadStringTaskAsync(requestUri);

                        if (!this.isClosed)
                        {
                            this.ParseGoldfishDeck(result);

                            this.DialogResult = true;
                            this.Close();
                            return;   
                        }
                    }

                    match = Regex.Match(uri.LocalPath, "/download/([0-9]+)");
                    if (match.Length > 0)
                    {
                        string result = await client.DownloadStringTaskAsync(uri);

                        if (!this.isClosed)
                        {
                            this.ParseGoldfishDeck(result);

                            this.DialogResult = true;
                            this.Close();
                            return;   
                        }
                    }

                    match = Regex.Match(uri.LocalPath, "/archetype/([A-Za-z\\-0-9]+)");
                    if (match.Length > 0)
                    {
                        string result = await client.DownloadStringTaskAsync(uri);

                        var bodyMatch = Regex.Match(result, "/deck/download/([0-9]+)");
                        var id = bodyMatch.Groups[1].Captures[0].ToString();

                        var requestUri = new Uri("http://" + uri.Host + "/deck/download/" + id + "/");
                        string decklistResult = await client.DownloadStringTaskAsync(requestUri);

                        if (!this.isClosed)
                        {
                            this.ParseGoldfishDeck(decklistResult);

                            this.DialogResult = true;
                            this.Close();
                            return;   
                        }
                    }

                    if (!this.isClosed)
                    {
                        MessageBox.Show("Incorrect deck URL.");   
                    }
                }
                else if (uri.Host == "www.mtgtop8.com")
                {
                    var match = Regex.Match(uri.LocalPath, "/event");
                    if (match.Length > 0)
                    {

                        var parsedQueryString = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        var id = parsedQueryString["d"];

                        if (id != null)
                        {
                            var requestUri = new Uri("http://www.mtgtop8.com/export_files/deck" + id + ".mwDeck");
                            string result = await client.DownloadStringTaskAsync(requestUri);

                            if (!this.isClosed)
                            {
                                this.ParseTop8Deck(result);

                                this.DialogResult = true;
                                this.Close();
                                return;   
                            }
                        }
                    }

                    match = Regex.Match(uri.LocalPath, "/export_files/deck([0-9]+).mwDeck");
                    if (match.Length > 0)
                    {
                        string result = await client.DownloadStringTaskAsync(uri);

                        if (!this.isClosed)
                        {
                            this.ParseTop8Deck(result);

                            this.DialogResult = true;
                            this.Close();
                            return;
                        }
                    }

                    if (!this.isClosed)
                    {
                        MessageBox.Show("Incorrect deck URL.");   
                    }
                }
                else if (uri.Host == "tappedout.net")
                {
                    var match = Regex.Match(uri.LocalPath, "/mtg-decks/([a-zA-Z0-9\\-]+)/");
                    if (match.Length > 0)
                    {
                        var requestUri = new Uri("http://www.tappedout.net/mtg-decks/" + match.Groups[1].Captures[0].ToString() + "/?fmt=txt");
                        string result = await client.DownloadStringTaskAsync(requestUri);

                        if (!this.isClosed)
                        {
                            this.ParseTappedOutDeck(result);

                            this.DialogResult = true;
                            this.Close();
                            return;   
                        }
                    }

                    MessageBox.Show("Incorrect deck URL.");
                }
                else
                {
                    MessageBox.Show("Unsupported website.");
                }
            }
            catch (WebException ex)
            {
                if (!isClosed)
                {
                    MessageBox.Show(
                    "Could not download the page from the web. This may mean the URL is not correct or that your computer is not connected to the internet.\n\nMessage: " + ex.Message);   
                }
            }
            catch (Exception ex)
            {
                if (!isClosed)
                {
                    MessageBox.Show(ex.ToString());   
                }
            }

            this.download.IsEnabled = true;
        }

        private void GetFromWebDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.deckUrl.Focus();
        }

        private async void DeckUrl_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Download_OnClick(sender, null);
            }
        }

        private void GetFromWebDialog_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void GetFromWebDialog_OnClosed(object sender, EventArgs e)
        {
            isClosed = true;
        }
    }
}
