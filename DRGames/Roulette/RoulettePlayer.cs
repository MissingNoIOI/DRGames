using DRGames.Games;

namespace DRGames.Roulette
{
	public sealed class RoulettePlayer : IPlayer
	{
		public string Name { get; set; }
		public string World { get; set; }
		public long Bet { get; set; }
		public bool IsPlaying { get; set; }
		public RouletteBet? RouletteBet { get; set; }
		public bool AutoBetCaptured { get; set; }
		public bool? Won { get; set; }
		public long Payout { get; set; }

		public RoulettePlayer(string name, string world)
		{
			Name = name;
			World = world;
		}
	}
}
