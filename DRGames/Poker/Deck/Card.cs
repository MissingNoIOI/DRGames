namespace DRGames.Poker.Deck
{
	public record Card(Rank Rank, Suit? Suite)
	{
		public string FullName => Suite == null ? Rank.Name : $"{Suite.Sign} {Rank.Name} of {Suite.Name} {Suite.Sign}";

		public override string ToString()
		{
			return FullName;
		}
	}
}
