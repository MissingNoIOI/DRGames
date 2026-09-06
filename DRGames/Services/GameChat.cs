using DRGames.Games;
using ECommons.Automation;

namespace DRGames.Services
{
	public sealed class GameChat : IGameChat
	{
		public void SendPartyMessage(string message)
		{
			Chat.SendMessage($"/p {message}");
		}

		public void SendTell(string playerName, string world, string message)
		{
			Chat.SendMessage($"/tell {playerName}@{world} {message}");
		}
	}
}
