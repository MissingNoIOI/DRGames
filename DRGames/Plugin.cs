using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DRGames.Poker;
using DRGames.Windows;
using System.IO;
using static DRGames.Helpers;

namespace DRGames
{
	public sealed class Plugin : IDalamudPlugin
	{
		[PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
		[PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
		[PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
		[PluginService] internal static IClientState ClientState { get; private set; } = null!;
		[PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
		[PluginService] internal static IDataManager DataManager { get; private set; } = null!;
		[PluginService] internal static IPluginLog Log { get; private set; } = null!;
		[PluginService] internal static IPartyList PartyList { get; private set; } = null!;
		[PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
		[PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;


		public string Name => "DRGames";
		private const string CommandName = "/drgames";

		public Configuration Configuration { get; init; }
		public PokerGame PokerGame { get; init; }
		public WindowSystem WindowSystem = new("DRGames");

		private ConfigWindow ConfigWindow { get; init; }

		private PokerWindow PokerWindow { get; init; }
		private MainWindow MainWindow { get; init; }

		public Plugin()
		{
			Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

			var pokerGame = new PokerGame(PartyList, ObjectTable);

			ConfigWindow = new ConfigWindow(this);
			PokerWindow = new PokerWindow(pokerGame, ChatGui);
			MainWindow = new MainWindow(this);

			Logger.Log = Log;

			WindowSystem.AddWindow(ConfigWindow);
			WindowSystem.AddWindow(PokerWindow);
			WindowSystem.AddWindow(MainWindow);

			_ = CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
			{
				HelpMessage = "Game collection to be used in the Dragon's Rest"
			});

			PluginInterface.UiBuilder.Draw += DrawUI;
			PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

			PluginInterface.UiBuilder.OpenMainUi += () => MainWindow.IsOpen = true;
		}

		public void Dispose()
		{
			WindowSystem.RemoveAllWindows();

			ConfigWindow.Dispose();
			MainWindow.Dispose();
			PokerWindow.Dispose();

			_ = CommandManager.RemoveHandler(CommandName);
		}

		private void OnCommand(string command, string args)
		{
			// in response to the slash command, just display our main ui
			MainWindow.IsOpen = true;
		}

		private void DrawUI()
		{
			WindowSystem.Draw();
		}

		public void DrawConfigUI()
		{
			ConfigWindow.IsOpen = true;
		}

		public void DrawPokerUI()
		{
			PokerWindow.IsOpen = true;
		}
	}
}
