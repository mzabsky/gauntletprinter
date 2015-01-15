using System.Collections.Generic;

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
}