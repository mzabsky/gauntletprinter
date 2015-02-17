using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace GauntletPrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private List<TextBox> deckInputs = new List<TextBox>();
        private List<TextBox> sideboardInputs = new List<TextBox>();

        public MainWindow()
        {
            InitializeComponent();

            this.deckInputs.Add(this.deck1);
            this.deckInputs.Add(this.deck2);
            this.deckInputs.Add(this.deck3);
            this.deckInputs.Add(this.deck4);
            this.deckInputs.Add(this.deck5);

            this.sideboardInputs.Add(this.sideboard1);
            this.sideboardInputs.Add(this.sideboard2);
            this.sideboardInputs.Add(this.sideboard3);
            this.sideboardInputs.Add(this.sideboard4);
            this.sideboardInputs.Add(this.sideboard5);
        }

        private void LoadFromFile(string path, int deckNumber)
        {
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                var decks = content.Split(new string[] { "\n==========\n", "\n\r==========\n\r", "\r\n==========\r\n" }, StringSplitOptions.None);
                for (int i = 0; i < decks.Length; i++)
                {
                    if (deckNumber + i < deckInputs.Count)
                    {
                        var deckString = decks[i].Trim();
                        
                        var sideboardSeparatorPosition = Math.Max(
                            deckString.LastIndexOf("\n\r", System.StringComparison.Ordinal),
                            deckString.LastIndexOf("\n\n", System.StringComparison.Ordinal));

                        if (sideboardSeparatorPosition != -1)
                        {
                            this.deckInputs[deckNumber + i].Text =
                                deckString.Substring(0, sideboardSeparatorPosition).Trim();
                            this.sideboardInputs[deckNumber + i].Text =
                                deckString.Substring(sideboardSeparatorPosition + 1).Trim();
                        }
                        else
                        {
                            this.deckInputs[deckNumber + i].Text = deckString;
                            this.sideboardInputs[deckNumber + i].Text = "";
                        }
                    }
                }

                this.generate.Focus();
            }
            else
            {
                MessageBox.Show("Input file \"" + path + "\" not found.");
            }
        }

        private void LoadFromFileWithDialog(int deckNumber)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.LoadFromFile(dialog.FileName, deckNumber);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                this.LoadFromFile(args[1], 0);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            //this.InvalidateVisual();

            // Give the window a chance to redraw
            await Task.Delay(TimeSpan.FromMilliseconds(1));

            try
            {
                var allCards = JsonConvert.DeserializeObject<Dictionary<string, Card>>(File.ReadAllText("AllCards.json", Encoding.UTF8)).Values.ToList();

                var additionalCards = new List<Card>();

                // Split, double faced and flip cards are represented as a separate left and right half in the source data
                foreach (var leftHalf in allCards.Where(p => p.Layout == "split" && p.Names.FirstOrDefault() == p.Name))
                {
                    var rightHalfName = leftHalf.Names.LastOrDefault();
                    var rightHalf = allCards.Single(p => p.Name == rightHalfName);

                    var splitCard = new Card
                    {
                        Name = leftHalf.Name + " // " + rightHalf.Name,
                        ManaCost = leftHalf.ManaCost + "//" + rightHalf.ManaCost,
                        Type = leftHalf.Type + "//" + rightHalf.Type,
                        Text = leftHalf.Text + "\n // " + rightHalf.Text
                    };

                    additionalCards.Add(splitCard);
                }

                foreach (var leftHalf in allCards.Where(p => p.Layout == "double-faced" && p.Names.FirstOrDefault() == p.Name))
                {
                    var rightHalfName = leftHalf.Names.LastOrDefault();
                    var rightHalf = allCards.Single(p => p.Name == rightHalfName);

                    leftHalf.Name = leftHalf.Name;
                    leftHalf.ManaCost = leftHalf.ManaCost;
                    leftHalf.Type = leftHalf.Type;
                    leftHalf.Text = "Transforms into " + rightHalf.Name + " [" + rightHalf.Type + ", " + rightHalf.Power + "/" + rightHalf.Toughness + "]; " + leftHalf.Text + "\n // " + (rightHalf.Text != null ? rightHalf.Text.Replace(rightHalf.Name, "~") : "");
                }

                foreach (var leftHalf in allCards.Where(p => p.Layout == "flip" && p.Names.FirstOrDefault() == p.Name))
                {
                    var rightHalfName = leftHalf.Names.LastOrDefault();
                    var rightHalf = allCards.Single(p => p.Name == rightHalfName);

                    leftHalf.Name = leftHalf.Name;
                    leftHalf.ManaCost = leftHalf.ManaCost;
                    leftHalf.Type = leftHalf.Type;
                    leftHalf.Text = "Flips to " + rightHalf.Name + " [" + rightHalf.Type + (rightHalf.Power != null ? ", " + rightHalf.Power + "/" + rightHalf.Toughness : "") + "]; " + leftHalf.Text + "\n // " + (rightHalf.Text != null ? rightHalf.Text.Replace(rightHalf.Name, "~") : "");
                }

                allCards.AddRange(additionalCards);

                // Parse the deck strings and shorten the text of cards that are included in one or more decks
                var shortener = new CardTextShortener();
                var decks = new List<List<Card>>();
                var sideboards = new List<List<Card>>();
                for (int deckNumber = 0; deckNumber < this.deckInputs.Count; deckNumber++)
                {
                    if (this.deckInputs[deckNumber].Text.Trim().Length == 0)
                    {
                        continue;
                    }

                    var deck = ParseDeckString(this.deckInputs[deckNumber].Text, allCards);
                    var sideboard = ParseDeckString(this.sideboardInputs[deckNumber].Text, allCards);

                    foreach (var card in deck.Concat(sideboard))
                    {
                        shortener.ProcessCard(card, this.grayscaleSymbols.IsChecked == true, this.omitTypeLineForBasics.IsChecked == true);
                    }

                    decks.Add(deck);
                    sideboards.Add(sideboard);
                }

                // Validate sizes of the decks
                if (decks.Any(p => p.Count != decks[0].Count))
                {
                    var numberOfValidDecks = decks.TakeWhile(p => p.Count == decks[0].Count).Count();
                    var invalidDeck = decks.First(p => p.Count != decks[0].Count);

                    throw new ApplicationException("All decks must contain the same number of cards (deck 1 contains " + decks[0].Count + " cards, deck " + (numberOfValidDecks + 1) + " contains " + invalidDeck.Count + " cards.)");
                }

                // Validate sizes of the sideboards
                if (sideboards.Any(p => p.Count != sideboards[0].Count))
                {
                    var numberOfValidDecks = sideboards.TakeWhile(p => p.Count == sideboards[0].Count).Count();
                    var invalidDeck = sideboards.First(p => p.Count != sideboards[0].Count);

                    throw new ApplicationException("All sideboards must contain the same number of cards (sideboard 1 contains " + sideboards[0].Count + " cards, sideboard " + (numberOfValidDecks + 1) + " contains " + invalidDeck.Count + " cards.)");
                }

                // Format the decks into HTML
                var arranger = new SimpleArranger();
                decks = arranger.ArrangeCards(decks);
                sideboards = arranger.ArrangeCards(sideboards);

                string str = @"
                <html>
                    <head>
                        <meta charset=""utf-8"">
                        <style>
                            .card
                            {
                                table-layout: fixed;
                                display:inline-block;
                                text-overflow:ellipsis;
                            }

                            .card > div {
                                width: 57mm;
                                height: 83mm;
                                border: 1px black solid;";

                            if(this.cardSpacing.IsChecked == true) str += @"margin: 0.5mm;";

                            str += @"}

                            .card table {
                                width: 100%;
                                height: 100%;
                                font-size: 3mm; /*11px;*/
                                padding: 0; 
                                border-spacing: 0;
                            }";

                            if(this.colorCode.IsChecked == true)
                            {
                                str += @"tr.card1 td{
                                    border-left: 4px #B2F1BA solid;
                                }

                                tr.card2 td{
                                    border-left: 4px #C3B5EB solid;
                                }

                                tr.card3 td{
                                    border-left: 4px #FFF4BC solid;
                                }

                                tr.card4 td{
                                    border-left: 4px #FFBFBC solid;
                                }

                                tr.card5 td{
                                    border-left: 4px #83EBE4 solid;
                                }";
                            }
                            

                            str += @".card td {
                                padding: 1px 1px 1px 1mm;
                            }

                            div.sideboard table 
                            {
                                border-top: 3px black dashed;
                            }

                            .cardNameRow {
                                height: 3.5mm;
                                white-space:nowrap;
                            }

                            .deckNumber {
                                float:left;
                                margin-right: 1mm;
                                /*border-radius: 50%;

                                width: 10px;
                                height: 10px;
                                padding: 1px;

                                background: #fff;
                                border: 2px solid #666;
                                color: #666;
                                text-align: center;*/
                            }

                            .cardName {
                                float:left;
                                font-weight: bold;
                                font-size: 1.1em;
                            }

                            .manaCost {
                                float:right;
                            }

                            .cardSeparator {
                                height:1px;
                                font-size: 1px;
                                padding: 0;
                            }

                            .cardSeparator hr{
                                border: 0;
                                border-top: 1px black solid;
                                background: none;
                            }

                            .cardTypeRow {
                                height: 3.5mm;       
                                /*white-space:nowrap;*/
                            }

                            .cardType {
                                float:left;
                            }

                            .powerToughness {
                                float:right;
                            }
                        </style>
                    </head>
                    <body>                
                ";

                var deckSize = decks.Max(p => p.Count);
                var totalSize = deckSize + sideboards.Max(p => p.Count);
                for (var i = 0; i < totalSize; i++)
                {
                    str += @"<div class=""card " + (i >= deckSize ? "sideboard" : "") + @""">
                                <div>
                                    <table>";

                    for (int j = 0; j < decks.Count; j++ )
                    {
                        var deck = decks[j].Concat(sideboards[j]).ToList();

                        if (deck.Count > i)
                        {
                            var card = deck[i];

                            if(j > 0)
                            {
                                str += @"
                                    <tr class=""cardSeparator card" + (j + 1) + @""">
                                        <td><hr /></td>
                                    </tr>";
                            }

                            str += @"<tr class=""cardNameRow card" + (j + 1) + @""">
                                <td>
                                    " +
                                   (this.deckNumbers.IsChecked == true
                                       ? @"<span class=""deckNumber"">" + (j + 1) + @"</span> "
                                       : "") + @"
                                    <span class=""cardName"">" + card.Name + @"</span> 
                                    <span class=""manaCost"">" + (card.ManaCost ?? "") + @"</span>
                                </td>
                            </tr>";

                            if (!(this.omitTypeLineForBasics.IsChecked == true && card.Type.Contains("Basic")))
                            {
                                str += @"<tr class=""cardTypeRow card" + (j + 1) + @""">
                                <td><span class=""cardType"">" + card.Type + @"</span> <span class=""powerToughness"">" + (card.Power != null ? card.Power + "/" + card.Toughness : (card.Loyalty ?? "")) + @"</span></td>
                                </tr>";
                            }
                            
                            /*if (card.Text != null)
                            {*/
                                str += @"<tr class=""cardText card" + (j + 1) + @""">
                                    <td>" + card.Text + @"</td>
                                </tr>";
                            /*}*/
                        }
                        else
                        {
                            str += @"<tr class=""cardNameRow"">
                                <td>-</td>
                            </tr>";
                        }
                    }

                    str += @"
                                
                                        <!--<tr></tr>-->
                                    </table>
                                </div>
                            </div>";
                }

                File.WriteAllText("cards.html", str, new UTF8Encoding(true));

                if (MessageBox.Show("File generated. Do you wish to open it?" + Environment.NewLine + Environment.NewLine + "WARNING: Internet Explorer does not display the cards correctly. Google Chrome is recommended to print the cards.", "Finished", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Process.Start("cards.html");
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.IsEnabled = true;
        }

        private static List<Card> ParseDeckString(string deckString, List<Card> allCards)
        {
            var deck = new List<Card>();

            var deckMatch = Regex.Match(deckString, @"\s*(\s*(?<count>\d*)(?<card>.*))*");

            for (int i = 0; i < deckMatch.Groups["card"].Captures.Count; i++)
            {
                var name = deckMatch.Groups["card"].Captures[i].ToString().Trim();

                if (name == "")
                {
                    continue;
                }

                var countString = deckMatch.Groups["count"].Captures[i].ToString() == ""
                    ? "1"
                    : deckMatch.Groups["count"].Captures[i].ToString().Trim();
                var count = int.Parse(countString);

                for (int j = 0; j < count; j++)
                {
                    var foundCard =
                        allCards.SingleOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (foundCard == null)
                    {
                        throw new ApplicationException("Card \"" + name + "\" not found.");
                    }
                    else
                    {
                        deck.Add(foundCard);
                    }
                }
            }
            return deck;
        }

        private void GetDeckFromWeb(int deckNumber)
        {
            var dialog = new GetFromWebDialog();
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            this.deckInputs[deckNumber].Text = dialog.DeckString;
            this.sideboardInputs[deckNumber].Text = dialog.SideboardString;
        }

        private void GetFromWeb1_OnClick(object sender, RoutedEventArgs e)
        {
            this.GetDeckFromWeb(0);
        }

        private void GetFromWeb2_OnClick(object sender, RoutedEventArgs e)
        {
            this.GetDeckFromWeb(1);
        }

        private void GetFromWeb3_OnClick(object sender, RoutedEventArgs e)
        {
            this.GetDeckFromWeb(2);
        }

        private void GetFromWeb4_OnClick(object sender, RoutedEventArgs e)
        {
            this.GetDeckFromWeb(3);
        }

        private void GetFromWeb5_OnClick(object sender, RoutedEventArgs e)
        {
            this.GetDeckFromWeb(4);
        }

        /*private void GetFromWeb6_OnClick(object sender, RoutedEventArgs e)
        {
            this.GetDeckFromWeb(this.deck6);
        }*/

        private void LoadFromFile1_OnClick(object sender, RoutedEventArgs e)
        {
            this.LoadFromFileWithDialog(0);
        }

        private void LoadFromFile2_OnClick(object sender, RoutedEventArgs e)
        {
            this.LoadFromFileWithDialog(1);
        }

        private void LoadFromFile3_OnClick(object sender, RoutedEventArgs e)
        {
            this.LoadFromFileWithDialog(2);
        }

        private void LoadFromFile4_OnClick(object sender, RoutedEventArgs e)
        {
            this.LoadFromFileWithDialog(3);
        }

        private void LoadFromFile5_OnClick(object sender, RoutedEventArgs e)
        {
            this.LoadFromFileWithDialog(4);
        }

        private async void UpdateCardData_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(
                    "GauntletPrinter will download latest card data from mtgjson.com. This operation requires internet connection and will download ~5 MB of data. It may take a moment.",
                    "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            this.IsEnabled = false;

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var requestUri = new Uri("http://mtgjson.com/json/AllCards.json");
            try
            {
                string result = await client.DownloadStringTaskAsync(requestUri);

                File.WriteAllText("AllCards.json", result, Encoding.UTF8);

                MessageBox.Show("Card data updated!");
            }
            catch (WebException)
            {
                MessageBox.Show("Could not download card data from the internet.");
            }
            catch (IOException)
            {
                MessageBox.Show("Could not save the downloaded data to the hard drive.");
            }

            this.IsEnabled = true;
        }
    }
}
