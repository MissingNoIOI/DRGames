using System.Collections.Immutable;

namespace DRGames.Poker.Deck
{
	public record Suit(string Name, char Sign)
	{
		public static readonly ImmutableList<Suit> suites = ImmutableList.Create(
	new Suit("Clubs", '♣'),
	new Suit("Diamonds", '♦'),
	new Suit("Hearts", '♥'),
	new Suit("Spades", '♠')
);

		public override string ToString()
		{
			return Name;
		}
	}
}
