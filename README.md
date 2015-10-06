# GauntletPrinter

GauntletPrinter is a new tool to print multiple decks on a single deck of proxies.

It is an application for Windows (.Net Framework 4.5 required).

**How does it work?** You fill in the decklists. The tool will then load all the cards, shorten their rules text as much as possible and then it arranges the cards so they fit onto the proxies. It generates a HTML file that can be printed using a web browser (Warning, it doesn't work with Internet Explorer, Google Chrome works best).

[Download application for Windows](http://puu.sh/krrxb/0f41f9baf4.zip)
-------------------------

Features
---------------------------
- Up to 5 decks printed on a single deck of proxies (4 generally fit quite easily, 5 are not guaranteed)
- Imports decklists straight from MTGGoldfish, MTGTop8 and TappedOut
- Optimizes the proxies either for colored or black-and-white printing
- Sideboard cards are printed on separate set of cards and visibly marked to be easily recognizable
- Powerful rules text shortening engine, which compacts the card text without significantly impacting its clarity

Sample gauntlets
---------------------------
These are gauntlets generated with this tool.

**Modern** ([colored PDF](http://puu.sh/g1eFU/f33d818329.pdf), [grayscale PDF](http://puu.sh/g1gEB/8f3c91b026.pdf)) - contains Brandom Remley's Junk, Oleg Plisov's Burn, Sebastian Pozzo's Affinity and Jason Schousboe's Tarmo-Twin.

**Duel Commander** ([colored PDF](http://puu.sh/g1edX/853d40b479.pdf), [grayscale PDF](http://puu.sh/g1gHI/d51b5b12b5.pdf)) - contains [icecoldjazz's Azusa](http://www.mtgsalvation.com/forums/the-game/commander-edh/forum-1-vs-1-commander-decklists/564451-azusa-lost-but-seeking-aka-ramp-dec), [hakz's Rafiq](http://forums.mtgsalvation.com/showthread.php?t=392838), [Leroy's Grand Arbiter Augustin](http://forums.mtgsalvation.com/showthread.php?t=497498) and [dawN's Exava](http://forums.mtgsalvation.com/showthread.php?t=341982).

Version History
------------------------------
**Version 1.4** (26.9.2015)
- Added several more rules text shortening rules, mostly to better support new ability words from DTK, Origins and BFZ.
- Fixed release package.

**Version 1.3** (14.9.2015)
- Fixed deck import from TappedOut sometimes not working.
- Updated data file for Origins.

**Version 1.2** (17.2.2015)
- Added online decks (MTGGoldfish, MTGTop8 and TappedOut supported)
- Added sideboards
- Added support for 5th deck
- Added option to omit type line for basic lands
- Improved the rules text shortening engine with 80 additional rules (upping the count to almost 300)

**Version 1.1** (15.1.2015)
- Added grayscale and color printing modes - colored mode has colored indentification strips while grayscale mode has numeric identifiers and special grayscale symbols

**Version 1.0** (14.1.2015)
- Initial release