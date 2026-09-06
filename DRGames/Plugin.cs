using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DRGames.Games;
using DRGames.Poker;
using DRGames.Services;
using DRGames.Windows;
using ECommons;
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

		private readonly ConfigWindow configWindow;

		private readonly PokerWindow pokerWindow;
		private readonly MainWindow mainWindow;
		private readonly IGameChat gameChat;

		public Plugin()
		{
			ECommonsMain.Init(PluginInterface, this);
			Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

			var pokerGame = new PokerGame(PartyList, ObjectTable);
			gameChat = new GameChat();

			configWindow = new ConfigWindow(this);
			pokerWindow = new PokerWindow(pokerGame, ChatGui, gameChat);
			mainWindow = new MainWindow(this);

			Logger.Log = Log;

			WindowSystem.AddWindow(configWindow);
			WindowSystem.AddWindow(pokerWindow);
			WindowSystem.AddWindow(mainWindow);

			_ = CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
			{
				HelpMessage = "Game collection to be used in the Dragon's Rest"
			});

			PluginInterface.UiBuilder.Draw += DrawUI;
			PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

			PluginInterface.UiBuilder.OpenMainUi += () => mainWindow.IsOpen = true;
		}

		public void Dispose()
		{
			WindowSystem.RemoveAllWindows();

			configWindow.Dispose();
			mainWindow.Dispose();
			pokerWindow.Dispose();

			_ = CommandManager.RemoveHandler(CommandName);
			ECommonsMain.Dispose();
		}

		private void OnCommand(string command, string args)
		{
			// in response to the slash command, just display our main ui
			mainWindow.IsOpen = true;
		}

		private void DrawUI()
		{
			WindowSystem.Draw();
		}

		public void DrawConfigUI()
		{
			configWindow.IsOpen = true;
		}

		public void DrawPokerUI()
		{
			pokerWindow.IsOpen = true;
		}
	}
}
