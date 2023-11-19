using DRGames.Poker.Deck;
using System;

namespace DRGames.Poker
{
	public class Player
	{
		public string Name { get; set; }
		public string World { get; set; }
		public Tuple<Card, Card>? Hand { get; set; }
		public int Chips { get; set; } = 0;

		public bool IsPlaying { get; set; } = false;

		public Player(string name, string world)
		{
			Name = name;
			World = world;
		}
	}
}
