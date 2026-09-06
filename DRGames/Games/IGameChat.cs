namespace DRGames.Games
{
	public interface IGameChat : System.IDisposable
	{
		void SendCommand(string command);
		void SendPartyMessage(string message);
		void SendTell(string playerName, string world, string message);
	}
}
