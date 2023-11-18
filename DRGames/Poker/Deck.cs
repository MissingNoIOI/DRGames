using System.Collections.Generic;
using System.Collections.Immutable;

namespace DRGames.Poker
{
	internal class Deck
	{
		private readonly ImmutableList<Rank> ranks = ImmutableList.Create(
			new Rank("Two", 2),
			new Rank("Three", 3),
			new Rank("Four", 4),
			new Rank("Five", 5),
			new Rank("Six", 6),
			new Rank("Seven", 7),
			new Rank("Eight", 8),
			new Rank("Nine", 9),
			new Rank("Ten", 10),
			new Rank("Joker", 11),
			new Rank("Queen", 12),
			new Rank("King", 13),
			new Rank("Ace", 14)
			);

		private readonly ImmutableList<Suit> suites = ImmutableList.Create(
			new Suit("Clubs", '♧'),
			new Suit("Diamonds", '♢'),
			new Suit("Hearts", '♡'),
			new Suit("Spades", '♤')
		);

		private Stack<Card> deck = null!;

		public bool IsDeckFresh => deck.Count < 52;

		public Deck()
		{
			GenerateNewDeck();
		}

		public void GenerateNewDeck()
		{
			var cards = new List<Card>();

			foreach (var suit in suites)
			{
				foreach (var rank in ranks)
				{
					cards.Add(new Card(rank, suit));
				}
			}

			cards.Shuffle();

			deck = new Stack<Card>(cards);
		}

		public Card? DrawCard()
		{
			return deck.Count > 0 ? deck.Pop() : null;
		}
	}
}
