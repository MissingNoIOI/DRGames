namespace DRGames.Poker.Deck
{
	public record Card(Rank Rank, Suit? Suite)
	{
		public string FullName => $"{GetShortRank()}{Suite?.Sign}";

		private string GetShortRank()
		{
			return Rank.Name switch
			{
				"Joker" => "J",
				"Queen" => "Q",
				"King" => "K",
				"Ace" => "A",
				_ => Rank.Name
			};
		}

		public override string ToString()
		{
			return FullName;
		}
	}
}
