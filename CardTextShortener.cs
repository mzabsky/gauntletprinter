using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Size = System.Windows.Size;

namespace GauntletPrinter
{
    public class CardTextShortener
    {
        private readonly Dictionary<string, string> rules = new Dictionary<string, string> {
            {"—", "-"},
            {"−", "-"},
            {"•", "&#9679;"},
            {"‘", "'"},
            {"less than or equal to", "<="},
            {"less than", "<"},
            {"greater than or equal to", ">="},
            {"greater than", ">"},
            {" plus ", " + "},
            {"equals to", "="},
            {"is equal to", "="},
            {"are equal to", "="},
            {"are each equal to", "="},
            {"equal to", "="},
            {"can't be 0", "&ne; 0"},
            {"\\n", "; "},
            {"enters the battlefield", "ETB"},
            {"enter the battlefield", "ETB"},
            {"graveyard", "GY"},
            {"mana pool", "MP"},
            {"counters on it", "counters"},
            {"\\+1/\\+1 counters", "{+1/+1}"},//
            {"\\+1/\\+1 counter", "{+1/+1}"},
            {"\\-1/\\-1 counters", "{-1/-1}"},
            {"\\-1/\\-1 counter", "{-1/-1}"},
            {"charge counters", "{charge}"},
            {"charge counter", "{charge}"},
            {"\\(.*?\\)", ""},
            {"opponent", "opp"},
            {"damage", "dmg"},
            {"Target", "Tgt"},
            {"target", "tgt"},
            {"converted mana cost", "CMC"},
            {"end of turn", "EOT"},
            {" one ", " 1 "},
            {" two ", " 2 "},
            {" three ", " 3 "},
            {" four ", " 4 "},
            {" five ", " 5 "},
            {" six ", " 6 "},
            {" seven ", " 7 "},
            {" eight ", " 8 "},
            {" nine ", " 9 "},
            {" ten ", " 10 "},
            {" eleven ", " 11 "},
            {" twelve ", " 12 "},
            {" thirteen ", " 13 "},
            {" fourteen ", " 14 "},
            {" fiveteen ", " 15 "},
            {" sixteen ", " 16 "},
            {" seventeen ", " 17 "},
            {" eighteen ", " 18 "},
            {" nineteen ", " 19 "},
            {" twenty ", " 20 "},
            {" one\\.", " 1."},
            {" two\\.", " 2."},
            {" three\\.", " 3."},
            {" four\\.", " 4."},
            {" five\\.", " 5."},
            {" six\\.", " 6."},
            {" seven\\.", " 7."},
            {" eight\\.", " 8."},
            {" nine\\.", " 9."},
            {" ten\\.", " 10."},
            {" eleven\\.", " 11."},
            {" twelve\\.", " 12."},
            {" thirteen\\.", " 13."},
            {" fourteen\\.", " 14."},
            {" fiveteen\\.", " 15."},
            {" sixteen\\.", " 16."},
            {" seventeen\\.", " 17."},
            {" eighteen\\.", " 18."},
            {" nineteen\\.", " 19."},
            {" twenty.", " 20."},
            {" one,", " 1,"},
            {" two,", " 2,"},
            {" three,", " 3,"},
            {" four,", " 4,"},
            {" five,", " 5,"},
            {" six,", " 6,"},
            {" seven,", " 7,"},
            {" eight,", " 8,"},
            {" nine,", " 9,"},
            {" ten,", " 10,"},
            {" eleven,", " 11,"},
            {" twelve,", " 12,"},
            {" thirteen,", " 13,"},
            {" fourteen,", " 14,"},
            {" fiveteen,", " 15,"},
            {" sixteen,", " 16,"},
            {" seventeen,", " 17,"},
            {" eighteen,", " 18,"},
            {" nineteen,", " 19,"},
            {" twenty,", " 20,"},
            {"First", "1st"},
            {"first", "1st"},
            {"second", "2nd"},
            {"third", "3rd"},
            {"fourth", "4th"},
            {" times ", "&#215; "},
            {" times.", "&#215;."},
            {"1 mana of any color", "{any}"},
            {"mana of any color", "{any}"},
            {"Draw a card, then discard a card", "Loot"},
            {"draw a card, then discard a card", "loot"},
            {"may draw a card. If you do, discard a card", "may loot"},
            {"sacrificed creature", "victim"},
            {"Sacrifice", "Sac"},
            {"sacrificed", "saced"},
            {"sacrifice", "sac"},
            {"draw a card", "draw 1"},
            {"Draw a card", "Draw 1"},
            {"Discard a card", "Discard 1"},
            {"discard a card", "discard 1"},
            {"discards a card", "discards 1"},
            {"countered by spells or abilities", "countered"},
            {"Activate this ability only any time you could cast a sorcery.", "Sorcery speed."},
            {"Activate this ability", "Activate this"},
            {"shuffles his or her library", "shuffles"},
            {"shuffle your library", "shuffle"},
            {"Shuffle your library", "Shuffle"},
            {"Search your library", "Tutor"},
            {"search your library", "tutor"},
            {"search a library", "tutor"},
            {"search his or her library", "tutor"},
            {"At the beginning of your upkeep", "At your upkeep"},
            {"at the beginning of your upkeep", "at your upkeep"},
            {"At the beginning of each upkeep", "At each upkeep"},
            {"at the beginning of each upkeep", "at each upkeep"},
            {"At the beginning of each player's upkeep", "At each upkeep"},
            {"at the beginning of each player's upkeep", "at each upkeep"},
            {"upkeep", "UPK"},
            {"creature cards", "creatures"},
            {"creature card", "creature"},
            {"instant cards", "instants"},
            {"instant card", "instant"},
            {"sorcery cards", "sorceries"},
            {"sorcery card", "sorcery"},
            {"enchantment cards", "enchantments"},
            {"enchantment card", "enchantment"},
            {"artifact cards", "artifacts"},
            {"artifact card", "artifact"},
            {"planeswalker cards", "planeswalkers"},
            {"planeswalker card", "planeswalker"},
            {"land cards", "lands"},
            {"land card", "land"},
            {"nonland cards", "nonlands"},
            {"nonland card", "nonland"},
            {"basic land", "basic"},
            {"nonbasic land", "nonbasic"},
            {"creature spells", "creatures"},
            {"creature spell", "creature"},
            {"instant spells", "instants"},
            {"instant spell", "instant"},
            {"sorcery spells", "sorceries"},
            {"sorcery spell", "sorcery"},
            {"enchantment spells", "enchantments"},
            {"enchantment spell", "enchantment"},
            {"artifact spells", "artifacts"},
            {"artifact spell", "artifact"},
            {"planeswalker spells", "planeswalkers"},
            {"planeswalker spell", "planeswalker"},
            {"land spells", "lands"},
            {"land spell", "land"},
            {"nonland spells", "nonlands"},
            {"nonland spell", "nonland"},
            {"that card", "it"},
            {"onto the battlefield", "into play"},
            {"on the battlefield", "in play"},
            {"leaves the battlefield", "leaves play"},
            {"token", "tkn"},
            {"As an additional cost to cast ~", "As an additional cost"},
            {"his or her", "his"},
            {"power and toughness", "P/T"},
            {" power ", " P "},
            {" toughness ", " T "},
            {"^;", ""},
            {"^ ;", ""},
            {" ;", ";"},
            {"Soulbond; As long as ~ is paired with another creature, both creatures have ", "Soulbond - "},
            {"Soulbond; As long as ~ is paired with another creature, both creatures get ", "Soulbond - "},
            {"Soulbond; As long as ~ is paired with another creature, each of those creatures gets ", "Soulbond - "},
            {"Soulbond; As long as ~ is paired with another creature, each of those creatures has ", "Soulbond - "},

            // Ability words
            {"Ferocious - ", ""},
            {"you control a creature with P 4 or greater", "ferocious"},
            {"Battalion - Whenever ~ and at least two other creatures attack, ", "Battalion - "},
            {"Channel - ", ""},
            {"Chroma - ", ""},
            {"Constellation - Whenever ~ or another enchantment ETB under your control, ", "Constellation - "},
            {"Domain - ", ""},
            {"to the number of basic land types among lands you control", "your domain"},
            {"Fateful hour - If you have 5 or less life, ", "Fateful hour - "},
            {"Grandeur - Discard another card named ~: ", "Grandeur - "},
            {"Hellbent - ", ""},
            {"you have no cards in hand", "hellbent"},
            {"Heroic - Whenever you cast a spell that targets ~, ", "Heroic - "},
            {"Imprint - ", ""},
            {"Inspired - Whenever ~ becomes untapped, ", "Inspired - "},
            {"Join forces - ", ""},
            {"Kinship - At the beginning of your upkeep, you may look at the top card of your library. If it shares a creature type with ~, you may reveal it. If you do, ", "Kinship - "},
            {"Landfall - Whenever a land ETB under your control, ", "Landfall - "},
            {"Landfall - If you had a land ETB under your control this turn, ", "Landfall - "},
            {"Lieutenant - As long as you control your commander, ", "Lieutenant - "},
            {"Metalcraft - ", ""},
            {"you control 3 or more artifacts", "metalcraft"},
            {"Morbid - ", ""},
            {"a creature died this turn", "morbid"},
            {"Radiance - ", ""},
            {"Raid - When ~ ETB, if you attacked with a creature this turn, ", "Raid - "},
            {"Raid - If you attacked with a creature this turn, ", "Raid - "},
            {"Sweep - Return any number of .* you control to their owner's hand. ", "Sweep - "},
            {"Threshold - ", ""},
            {"7 or more cards are in your graveyard", "threshold"},
            {"Will of the council - ", ""},
            {"Rally - Whenever ~ or another Ally ETB under your control, ", "Rally - "},
            {"Spell mastery - If there are >= 2 instant and/or sorcery cards in your GY, ", "Spell mastery - "},
            {"Formidable - ", ""},
            {"Converge - ", ""},
        };

        private readonly Dictionary<string, MatchEvaluator> advancedRules;
        
        // This is in string form so that it can be easily copied from MTG rules. Make sure to remove any " and ".
        private readonly string subtypes =
            "Contraption, Equipment, Fortification, Aura, Curse, Shrine, Desert, Forest, Island, Lair, Locus, Mine, Mountain, Plains, Power-Plant, Swamp, Tower, Urza's, Ajani, Ashiok, Bolas, Chandra, Dack, Domri, Elspeth, Garruk, Gideon, Jace, Karn, Kiora, Koth, Liliana, Nissa, Ral, Sarkhan, Sorin, Tamiyo, Tezzeret, Tibalt, Venser, Vraska, Xenagos, Arcane, Trap, Advisor, Ally, Angel, Anteater, Antelope, Ape, Archer, Archon, Artificer, Assassin, Assembly-Worker, Atog, Aurochs, Avatar, Badger, Barbarian, Basilisk, Bat, Bear, Beast, Beeble, Berserker, Bird, Blinkmoth, Boar, Bringer, Brushwagg, Camarid, Camel, Caribou, Carrier, Cat, Centaur, Cephalid, Chimera, Citizen, Cleric, Cockatrice, Construct, Coward, Crab, Crocodile, Cyclops, Dauthi, Demon, Deserter, Devil, Djinn, Dragon, Drake, Dreadnought, Drone, Druid, Dryad, Dwarf, Efreet, Elder, Eldrazi, Elemental, Elephant, Elf, Elk, Eye, Faerie, Ferret, Fish, Flagbearer, Fox, Frog, Fungus, Gargoyle, Germ, Giant, Gnome, Goat, Goblin, God, Golem, Gorgon, Graveborn, Gremlin, Griffin, Hag, Harpy, Hellion, Hippo, Hippogriff, Homarid, Homunculus, Horror, Horse, Hound, Human, Hydra, Hyena, Illusion, Imp, Incarnation, Insect, Jellyfish, Juggernaut, Kavu, Kirin, Kithkin, Knight, Kobold, Kor, Kraken, Lamia, Lammasu, Leech, Leviathan, Lhurgoyf, Licid, Lizard, Manticore, Masticore, Mercenary, Merfolk, Metathran, Minion, Minotaur, Monger, Mongoose, Monk, Moonfolk, Mutant, Myr, Mystic, Naga, Nautilus, Nephilim, Nightmare, Nightstalker, Ninja, Noggle, Nomad, Nymph, Octopus, Ogre, Ooze, Orb, Orc, Orgg, Ouphe, Ox, Oyster, Pegasus, Pentavite, Pest, Phelddagrif, Phoenix, Pincher, Pirate, Plant, Praetor, Prism, Rabbit, Rat, Rebel, Reflection, Rhino, Rigger, Rogue, Sable, Salamander, Samurai, Sand, Saproling, Satyr, Scarecrow, Scorpion, Scout, Serf, Serpent, Shade, Shaman, Shapeshifter, Sheep, Siren, Skeleton, Slith, Sliver, Slug, Snake, Soldier, Soltari, Spawn, Specter, Spellshaper, Sphinx, Spider, Spike, Spirit, Splinter, Sponge, Squid, Squirrel, Starfish, Surrakar, Survivor, Tetravite, Thalakos, Thopter, Thrull, Treefolk, Triskelavite, Troll, Turtle, Unicorn, Vampire, Vedalken, Viashino, Volver, Wall, Warrior, Weird, Werewolf, Whale, Wizard, Wolf, Wolverine, Wombat, Worm, Wraith, Wurm, Yeti, Zombie, Zubera";

        private List<string> parsedSubtypes;

        public CardTextShortener()
        {
            parsedSubtypes = this.subtypes.Split(',').Select(p => p.Trim()).ToList();
            
            advancedRules = new Dictionary<string, MatchEvaluator>
            {
                { "draw ([0-9]+|X) cards", m => "draw " + m.Groups[1].ToString() },
                { "Draw ([0-9]+|X) cards", m => "Draw " + m.Groups[1].ToString() },
                { "draws ([0-9]+|X) cards", m => "draws " + m.Groups[1].ToString() },
                { "discards ([0-9]+|X) cards", m => "discards " + m.Groups[1].ToString() },
                { "discard ([0-9]+|X) cards", m => "discard " + m.Groups[1].ToString() },
                { "Discard ([0-9]+|X) cards", m => "Discard " + m.Groups[1].ToString() },
                { "gain ([0-9]+|X) life", m => "gain " + m.Groups[1].ToString() },
                { "Gain ([0-9]+|X) life", m => "Gain " + m.Groups[1].ToString() },
                { "gains ([0-9]+|X) life", m => "gains " + m.Groups[1].ToString() },
                { "deals ([0-9]+|X) dmg", m => "deals " + m.Groups[1].ToString() },
                { "Lose ([0-9]+|X) life", m => "Lose " + m.Groups[1].ToString() },
                { "lose ([0-9]+|X) life", m => "lose " + m.Groups[1].ToString() },
                { "loses ([0-9]+|X) life", m => "loses " + m.Groups[1].ToString() },
                { "prevent the next ([0-9]+|X) dmg", m => "prevent next " + m.Groups[1].ToString() },
                { "Prevent the next ([0-9]+|X) dmg", m => "Prevent next " + m.Groups[1].ToString() },
                { "puts the top ([0-9]+|X) cards of his or her library into his or her graveyard", m => "mills " + m.Groups[1].ToString() },
                { "put the top ([0-9]+|X) cards of your library into your graveyard", m => "mill " + m.Groups[1].ToString() },
                { "reveals cards from the top of his or her library until he or she reveals ([0-9]+|X) land cards, then puts all cards revealed this way into his or her graveyard", m => "grinds " + m.Groups[1].ToString() },
                { "reveals cards from the top of his or her library until ([0-9]+|X) land cards are revealed. That player puts all cards revealed this way into his or her graveyard", m => "grinds " + m.Groups[1].ToString() },
                { "reveal cards from the top of your library until you reveal ([0-9]+|X) land cards, then put all cards revealed this way into your graveyard", m => "grind " + m.Groups[1].ToString() },
                { "Reveal cards from the top of your library until you reveal ([0-9]+|X) land cards, then put all cards revealed this way into your graveyard", m => "Grind " + m.Groups[1].ToString() },
                { "Return (.*?) to its owner's hand", m => "Bounce " + m.Groups[1].ToString() },
                { "return (.*?) to its owner's hand", m => "bounce " + m.Groups[1].ToString() },
                { "Bloodrush - (.*?), Discard [^;]+", m => "Bloodrush " + m.Groups[1].ToString()},
                { "Parley - Each player reveals the top card of his or her library. For each nonland card revealed this way, (.*?) Then each player draws a card.", m => "Parley - " + m.Groups[1].ToString()},
                { "Strive - ~ costs (.*?) more to cast for each target beyond the 1st.", (m) => "Strive " + m.Groups[1].ToString()},
                { "Tempting offer - ([^.]*?)\\..*", m => "Tempting offer - " + m.Groups[1].ToString() + ". "},
                { "([0-9]+|X) or more", m => ">= " + m.Groups[1].ToString() },
                { "([0-9]+|X) or less", m => "<= " + m.Groups[1].ToString() },
                { "([0-9]+|X) or fewer", m => "<= " + m.Groups[1].ToString() },
                { "up to ([0-9]+|X)", m => "<= " + m.Groups[1].ToString() },
                { "more than ([0-9]+|X)", m => "> " + m.Groups[1].ToString() },
                { "less than ([0-9]+|X)", m => "< " + m.Groups[1].ToString() },
                { "([A-Z]\\w+) cards", m => parsedSubtypes.Contains(m.Groups[1].ToString()) ? m.Groups[1].ToString() + "s" : m.Groups[1].ToString() + " cards"}, // These two rules need to be careful, because incorrect string could be matched (eg. "Draw cards equal to...")
                { "([A-Z]\\w+) card", m => parsedSubtypes.Contains(m.Groups[1].ToString()) ? m.Groups[1].ToString() : m.Groups[1].ToString() + " card" }
            };
        }

        private string ReplaceSymbols(string str, bool isGrayscale)
        {
            return isGrayscale ? this.ReplaceSymbolsGrayscale(str) : this.ReplaceSymbolsColored(str);
        }

        private string ReplaceSymbolsColored(string str)
        {
            str = str.Replace("{0}", @"<img src=""symbols/mana-0.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{1}", @"<img src=""symbols/mana-1.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2}", @"<img src=""symbols/mana-2.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{3}", @"<img src=""symbols/mana-3.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{4}", @"<img src=""symbols/mana-4.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{5}", @"<img src=""symbols/mana-5.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{6}", @"<img src=""symbols/mana-6.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{7}", @"<img src=""symbols/mana-7.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{8}", @"<img src=""symbols/mana-8.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{9}", @"<img src=""symbols/mana-9.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{10}", @"<img src=""symbols/mana-10.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{11}", @"<img src=""symbols/mana-11.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{12}", @"<img src=""symbols/mana-12.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{13}", @"<img src=""symbols/mana-13.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{14}", @"<img src=""symbols/mana-14.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{15}", @"<img src=""symbols/mana-15.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{16}", @"<img src=""symbols/mana-16.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{17}", @"<img src=""symbols/mana-17.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{18}", @"<img src=""symbols/mana-18.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{19}", @"<img src=""symbols/mana-19.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{20}", @"<img src=""symbols/mana-20.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{X}", @"<img src=""symbols/mana-x.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{Y}", @"<img src=""symbols/mana-y.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{Z}", @"<img src=""symbols/mana-z.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W}", @"<img src=""symbols/mana-w.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U}", @"<img src=""symbols/mana-u.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B}", @"<img src=""symbols/mana-b.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R}", @"<img src=""symbols/mana-r.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G}", @"<img src=""symbols/mana-g.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/W}", @"<img src=""symbols/mana-2w.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/U}", @"<img src=""symbols/mana-2u.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/B}", @"<img src=""symbols/mana-2b.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/R}", @"<img src=""symbols/mana-2r.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/G}", @"<img src=""symbols/mana-2g.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W/P}", @"<img src=""symbols/mana-pw.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U/P}", @"<img src=""symbols/mana-pu.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B/P}", @"<img src=""symbols/mana-pb.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R/P}", @"<img src=""symbols/mana-pr.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G/P}", @"<img src=""symbols/mana-pg.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W/U}", @"<img src=""symbols/mana-wu.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W/B}", @"<img src=""symbols/mana-wb.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U/B}", @"<img src=""symbols/mana-ub.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U/R}", @"<img src=""symbols/mana-ur.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B/R}", @"<img src=""symbols/mana-br.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B/G}", @"<img src=""symbols/mana-bg.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R/W}", @"<img src=""symbols/mana-rw.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R/G}", @"<img src=""symbols/mana-rg.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G/W}", @"<img src=""symbols/mana-gw.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G/U}", @"<img src=""symbols/mana-gu.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{T}", @"<img src=""symbols/tap.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{Q}", @"<img src=""symbols/untap.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{+1/+1}", @"<img src=""symbols/counter-plus.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{-1/-1}", @"<img src=""symbols/counter-minus.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{charge}", @"<img src=""symbols/counter-charge.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{any}", @"<img src=""symbols/mana-any.svg"" width=""10"" height=""10"" />");
            return str;
        }

        private string ReplaceSymbolsGrayscale(string str)
        {
            str = str.Replace("{0}", @"<img src=""symbols/mana-0.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{1}", @"<img src=""symbols/mana-1.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2}", @"<img src=""symbols/mana-2.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{3}", @"<img src=""symbols/mana-3.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{4}", @"<img src=""symbols/mana-4.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{5}", @"<img src=""symbols/mana-5.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{6}", @"<img src=""symbols/mana-6.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{7}", @"<img src=""symbols/mana-7.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{8}", @"<img src=""symbols/mana-8.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{9}", @"<img src=""symbols/mana-9.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{10}", @"<img src=""symbols/mana-10.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{11}", @"<img src=""symbols/mana-11.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{12}", @"<img src=""symbols/mana-12.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{13}", @"<img src=""symbols/mana-13.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{14}", @"<img src=""symbols/mana-14.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{15}", @"<img src=""symbols/mana-15.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{16}", @"<img src=""symbols/mana-16.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{17}", @"<img src=""symbols/mana-17.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{18}", @"<img src=""symbols/mana-18.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{19}", @"<img src=""symbols/mana-19.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{20}", @"<img src=""symbols/mana-20.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{X}", @"<img src=""symbols/mana-x.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{Y}", @"<img src=""symbols/mana-y.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{Z}", @"<img src=""symbols/mana-z.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W}", @"<img src=""symbols/mana-w-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U}", @"<img src=""symbols/mana-u-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B}", @"<img src=""symbols/mana-b-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R}", @"<img src=""symbols/mana-r-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G}", @"<img src=""symbols/mana-g-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/W}", @"<img src=""symbols/mana-2w-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/U}", @"<img src=""symbols/mana-2u-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/B}", @"<img src=""symbols/mana-2b-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/R}", @"<img src=""symbols/mana-2r-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{2/G}", @"<img src=""symbols/mana-2g-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W/P}", @"<img src=""symbols/mana-pw-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U/P}", @"<img src=""symbols/mana-pu-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B/P}", @"<img src=""symbols/mana-pb-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R/P}", @"<img src=""symbols/mana-pr-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G/P}", @"<img src=""symbols/mana-pg-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W/U}", @"<img src=""symbols/mana-wu-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{W/B}", @"<img src=""symbols/mana-wb-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U/B}", @"<img src=""symbols/mana-ub-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{U/R}", @"<img src=""symbols/mana-ur-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B/R}", @"<img src=""symbols/mana-br-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{B/G}", @"<img src=""symbols/mana-bg-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R/W}", @"<img src=""symbols/mana-rw-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{R/G}", @"<img src=""symbols/mana-rg-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G/W}", @"<img src=""symbols/mana-gw-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{G/U}", @"<img src=""symbols/mana-gu-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{T}", @"<img src=""symbols/tap.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{Q}", @"<img src=""symbols/untap.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{+1/+1}", @"<img src=""symbols/counter-plus.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{-1/-1}", @"<img src=""symbols/counter-minus.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{charge}", @"<img src=""symbols/counter-charge-gray.svg"" width=""10"" height=""10"" />");
            str = str.Replace("{any}", @"<img src=""symbols/mana-any.svg"" width=""10"" height=""10"" />");
            return str;
        }

        public void ProcessCard(Card card, bool isGrayscale, bool omitTypeLineForBasics)
        {
            if (card.Text != null && !card.Processed)
            {
                card.Text = card.Text.Replace(card.Name, "~");

                // The card can refer to itself with partial name (eg. Purphoros instead of Purphoros, God of the Forge)
                if (card.Name.Contains(","))
                {
                    var shortName =
                        card.Name.Substring(0, card.Name.IndexOf(",", System.StringComparison.Ordinal)).Trim();
                    card.Text = card.Text.Replace(shortName, "~");
                }

                foreach (var rule in rules)
                {
                    card.Text = Regex.Replace(card.Text, rule.Key, rule.Value);
                }

                foreach (var rule in advancedRules)
                {
                    card.Text = Regex.Replace(card.Text, rule.Key, rule.Value);
                }

                card.Type = card.Type.Replace('—', '-');

                /*if (omitTypeLineForBasics && card.Types.Contains("Basic"))
                {
                    card.TextLength = 0; // Title
                }
                else if (card.Text.Length == 0)
                {
                    card.TextLength = 14; // Title + type line 
                }
                else
                {
                    var regex = new Regex("{(.*?)}");
                    Font font = new Font("Arial", 8);
                    System.Drawing.Size textSize = TextRenderer.MeasureText(regex.Replace(card.Text, "O"), font, new System.Drawing.Size(210, 999), TextFormatFlags.WordBreak | TextFormatFlags.NoPadding | TextFormatFlags.Left);

                    card.TextLength = 14 + textSize.Height; // Title + type line + rules text
                }

                card.ManaCost += " " + card.TextLength;*/

                card.TextLength = (int)Math.Ceiling(card.Text.Length / 40.0);

                if (omitTypeLineForBasics && card.Types.Contains("Basic"))
                {
                    card.TextLength = 0;
                }

                card.Processed = true;

                //if (card.Name.Contains("Delver")) Debugger.Break();

                card.Text = this.ReplaceSymbols(card.Text, isGrayscale);
            }

            card.Type = card.Type.Replace("—", "-");  

            if (card.ManaCost != null)
            {
                card.ManaCost = this.ReplaceSymbols(card.ManaCost, isGrayscale);
            }  
        }
    }
}
