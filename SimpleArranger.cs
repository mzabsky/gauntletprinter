using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletPrinter
{
    public class SimpleArranger
    {
        internal class CardInDeck
        {
            public Card Card { get; set; }
            public int DeckNumber { get; set; }
        }

        public List<List<Card>> ArrangeCards(List<List<Card>> decks)
        {
            var data = new List<CardInDeck>();
            for(int i = 0; i < decks.Count; i++)
            {
                foreach(var card in decks[i])
                {
                    data.Add(new CardInDeck { Card = card, DeckNumber = i });
                }
            }

            var orderedData = data.OrderByDescending(p => p.Card.TextLength);

            int deckSize = decks[0].Count;
            Card[,] generatedDecks = new Card[decks.Count, deckSize];
            int cardIndex = 0;
            foreach(var cardInDeck in orderedData)
            {
                // Find deck slot with the most empty room AND empty slot for the card
                int foundSlot = 0;
                int foundFullness = int.MaxValue;
                for (int i = 0; i < deckSize; i++)
                {
                    if (generatedDecks[cardInDeck.DeckNumber, i] != null) continue;

                    int currentFullness = Enumerable.Range(0, decks.Count).Sum(p => generatedDecks[p, i] != null ? generatedDecks[p, i].TextLength : 0);
                    if(currentFullness == 0)
                    {
                        foundFullness = 0;
                        foundSlot = i;
                        break;
                    }
                    else if(currentFullness < foundFullness)
                    {
                        foundSlot = i;
                        foundFullness = currentFullness;
                    }
                }

                if (generatedDecks[cardInDeck.DeckNumber, foundSlot] != null) throw new ApplicationException("Could not arrange cards.");
                generatedDecks[cardInDeck.DeckNumber, foundSlot] = cardInDeck.Card;
            }

            var outputDecks = new List<List<Card>>();
            for (int i = 0; i < decks.Count; i++)
            {
                var deck = new List<Card>();
                for (int j = 0; j < deckSize; j++)
                {
                    deck.Add(generatedDecks[i, j]);
                }

                outputDecks.Add(deck);
            }
            return outputDecks;
        }
    }
}
