namespace DRGames.Games
{
	public interface IPlayer
	{
		string Name { get; }
		string World { get; }
		long Bet { get; set; }
		bool IsPlaying { get; set; }
	}
}
