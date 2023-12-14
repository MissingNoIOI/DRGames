using DRGames.Poker.Deck;
using System.Collections.Generic;

namespace DRGames.Poker.Solver
{
	internal class ApplicationUser
	{
		public string? Name { get; set; }
		public int Chips { get; set; }
		public List<Card>? PlayerCards { get; set; }
		public bool IsPlayingThisRound { get; set; } = true;
		public bool IsPlayingThisGame { get; set; } = true;
	}
}
