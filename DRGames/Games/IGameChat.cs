namespace DRGames.Games
{
	public interface IGameChat
	{
		void SendPartyMessage(string message);
		void SendTell(string playerName, string world, string message);
	}
}
