using System.Collections.Generic;

namespace DRGames.Games
{
	public interface IGame
	{
		IReadOnlyList<IPlayer> Players { get; }
		void Update();
		void RecordBet(string playerName, long amount);
		void EndGame();
	}
}
