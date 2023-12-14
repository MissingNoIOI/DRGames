using System.Collections.Immutable;
using System.Linq;

namespace DRGames.Poker.Deck
{
	public record Rank(string Name, int Value)
	{
		public static readonly ImmutableList<Rank> Ranks = ImmutableList.Create(
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

		public static Rank GetRank(int Value)
		{
			return Value is < 2 or > 14
				? throw new System.Exception("Got an invalid value for the rank of " + Value.ToString())
				: Ranks.First(r => r.Value == Value);
		}
		public static Rank GetRank(double Value)
		{
			return GetRank((int)Value);
		}
	}
}
