using System.Collections.Generic;
using static DRGames.Helpers;

namespace DRGames.Poker.Deck
{
	internal class CardDeck
	{
		private Stack<Card> deck = null!;

		public bool IsDeckFresh => deck.Count == 52;

		public CardDeck()
		{
			GenerateNewDeck();
		}

		public void GenerateNewDeck()
		{
			var cards = new List<Card>();

			foreach (var suit in Suit.suites)
			{
				foreach (var rank in Rank.Ranks)
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
