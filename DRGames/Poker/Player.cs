using DRGames.Poker.Deck;
using DRGames.Games;
using System;

namespace DRGames.Poker
{
	public class Player : IPlayer
	{
		public string Name { get; set; }
		public string World { get; set; }
		public Tuple<Card, Card>? Hand { get; set; }
		public int Chips { get; set; } = 0;
		public long Bet { get; set; } = 0;
		public long RoundBet { get; set; } = 0;

		public bool IsPlaying { get; set; } = false;
		public PokerAction Action { get; set; } = PokerAction.None;
		public bool ChatActionCaptured { get; set; }

		public Player(string name, string world)
		{
			Name = name;
			World = world;
		}
	}
}
