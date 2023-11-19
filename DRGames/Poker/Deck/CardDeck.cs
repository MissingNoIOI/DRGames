using System.Collections.Generic;
using System.Collections.Immutable;
using static DRGames.Helpers;

namespace DRGames.Poker.Deck
{
	internal class CardDeck
	{
		private readonly ImmutableList<Rank> ranks = ImmutableList.Create(
			new Rank("2", 2),
			new Rank("3", 3),
			new Rank("3", 4),
			new Rank("5", 5),
			new Rank("6", 6),
			new Rank("7", 7),
			new Rank("8", 8),
			new Rank("9", 9),
			new Rank("10", 10),
			new Rank("Joker", 11),
			new Rank("Queen", 12),
			new Rank("King", 13),
			new Rank("Ace", 14)
			);

		private readonly ImmutableList<Suit> suites = ImmutableList.Create(
			new Suit("Clubs", '♣'),
			new Suit("Diamonds", '♦'),
			new Suit("Hearts", '♥'),
			new Suit("Spades", '♠')
		);

		private Stack<Card> deck = null!;

		public bool IsDeckFresh => deck.Count == 52;

		public CardDeck()
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

		public Card DrawCard()
		{
			if (deck.Count == 0)
			{
				Logger.Log.Error("Tried to draw a card from an empty deck, generating new deck");
				GenerateNewDeck();
			}
			return deck.Pop();
		}
	}
}
