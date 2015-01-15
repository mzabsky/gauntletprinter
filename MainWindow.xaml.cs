using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GauntletPrinter
{
    public class Card
    {
        public string Layout { get; set; }
        public string Name { get; set; }
        public string ManaCost { get; set; }
        public string Type { get; set; }
        public List<string> Types { get; set; }
        public List<string> Subtypes { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string Loyalty { get; set; }
        public string ImageName { get; set; }
        public List<string> Names { get; set; }

        public int TextLength { get; set; }

        public bool Processed { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allCards = JsonConvert.DeserializeObject<Dictionary<string, Card>>(File.ReadAllText("AllCards.json")).Values.ToList();

                List<Card> additionalCards = new List<Card>();

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
                    leftHalf.Text = "Transforms to " + rightHalf.Name + " [" + rightHalf.Type + ", " + rightHalf.Power + "/" + rightHalf.Toughness + "]; " + leftHalf.Text + "\n // " + (rightHalf.Text != null ? rightHalf.Text.Replace(rightHalf.Name, "~") : "");
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

                var deckStrings = new List<string>();
                if (this.deck1.Text != "") deckStrings.Add(this.deck1.Text);
                if (this.deck2.Text != "") deckStrings.Add(this.deck2.Text);
                if (this.deck3.Text != "") deckStrings.Add(this.deck3.Text);
                if (this.deck4.Text != "") deckStrings.Add(this.deck4.Text);
            
                if(deckStrings.Count == 0)
                {
                    MessageBox.Show("You have to enter at least one deck.");
                    return;
                }

                // Parse the deck strings and shorten the text of cards that are included in one or more decks
                var shortener = new CardTextShortener();
                var decks = new List<List<Card>>();
                foreach (var deckString in deckStrings)
                {
                    var deck = new List<Card>();

                    var match = Regex.Match(deckString, @"\s*(\s*(?<count>\d*)(?<card>.*))*");

                    for (int i = 0; i < match.Groups["card"].Captures.Count; i++)
                    {
                        var name = match.Groups["card"].Captures[i].ToString().Trim();

                        if (name == "")
                        {
                            continue;
                        }

                        var countString = match.Groups["count"].Captures[i].ToString() == "" ? "1" : match.Groups["count"].Captures[i].ToString().Trim();
                        var count = int.Parse(countString);

                        for (int j = 0; j < count; j++)
                        {
                            var foundCard = allCards.SingleOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

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

                    foreach (var card in deck)
                    {
                        shortener.ProcessCard(card);
                    }

                    decks.Add(deck);
                }

                // Validate sizes of the decks
                if (decks.Any(p => p.Count != decks[0].Count))
                {
                    var numberOfValidDecks = decks.TakeWhile(p => p.Count == decks[0].Count).Count();
                    var invalidDeck = decks.First(p => p.Count != decks[0].Count);

                    throw new ApplicationException("All decks must contain the same number of cards (deck 1 contains " + decks[0].Count + " cards, deck " + (numberOfValidDecks + 1) + " contains " + invalidDeck.Count + " cards.)");
                }

                // Format the decks into HTML
                var arranger = new SimpleArranger();
                decks = arranger.ArrangeCards(decks);

                string str = @"
                <html>
                    <head>
                        <style>
                            .card
                            {
                                table-layout: fixed;
                                display:inline-block;
                                text-overflow:ellipsis;
                            }

                            .card > div {
                                width: 56mm;
                                height: 82mm;
                                border: 1px black solid;
                                margin: 0.5mm;              
                            }

                            .card table {
                                width: 100%;
                                height: 100%;
                                font-size: 3mm;
                                padding: 0.5mm;   
                            }

                            .card td {
                                padding: 1px;
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
                                border-top: 1px black solid;
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
                for (var i = 0; i < deckSize; i++)
                {
                    str += @"<div class=""card"">
                                <div>
                                    <table>";

                    for (int j = 0; j < decks.Count; j++ )
                    {
                        var deck = decks[j];

                        if (deck.Count > i)
                        {
                            var card = deck[i];

                            str += @"
                            <tr class=""cardNameRow"">
                                <td class=" + (deck != decks.FirstOrDefault() ? "cardSeparator" : "") + @">
                                    <span class=""deckNumber"">" + (j + 1) + @"</span> <span class=""cardName"">" + card.Name + @"</span> <span class=""manaCost"">" + (card.ManaCost != null ? card.ManaCost : "") + @"</span></td>
                            </tr>
                            <tr class=""cardTypeRow"">
                                <td><span class=""cardType"">" + card.Type + @"</span> <span class=""powerToughness"">" + (card.Power != null ? card.Power + "/" + card.Toughness : (card.Loyalty != null ? card.Loyalty.ToString() : "")) + @"</span></td>
                            </tr>";

                            if (card.Text != null)
                            {
                                str += @"<tr>
                                    <td>" + card.Text + @"</td>
                                </tr>";
                            }
                        }
                        else
                        {
                            str += @"<tr class=""cardNameRow"">
                                <td>-</td>
                            </tr>";
                        }
                    }

                    str += @"
                                
                                        <tr></tr>
                                    </table>
                                </div>
                            </div>";
                }

                File.WriteAllText("cards.html", str);

                if (MessageBox.Show("File generated. Do you wish to open it?" + Environment.NewLine + Environment.NewLine + "WARNING: Internet Explorer does not display the cards correctly. Google Chrome is recommended to print the cards.", "Finished", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Process.Start("cards.html");
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }
    }
}
