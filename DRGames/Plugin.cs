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
		public string Name => "DRGames";
		private const string CommandName = "/drgames";

		private DalamudPluginInterface PluginInterface { get; init; }
		private ICommandManager CommandManager { get; init; }
		private IPartyList PartyList { get; init; }
		private IChatGui ChatGui { get; init; }
		private IClientState ClientState { get; init; }
		public Configuration Configuration { get; init; }
		public PokerGame PokerGame { get; init; }
		public WindowSystem WindowSystem = new("DRGames");

		private ConfigWindow ConfigWindow { get; init; }

		private PokerWindow PokerWindow { get; init; }
		private MainWindow MainWindow { get; init; }

		public Plugin(
			[RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
			[RequiredVersion("1.0")] ICommandManager commandManager,
			[RequiredVersion("1.0")] IClientState clientState,
			[RequiredVersion("1.0")] IPluginLog pluginLog,
			[RequiredVersion("1.0")] IPartyList partyList,
			[RequiredVersion("1.0")] IChatGui chatGui)
		{
			PluginInterface = pluginInterface;
			CommandManager = commandManager;
			ClientState = clientState;
			PartyList = partyList;
			ChatGui = chatGui;

			Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
			Configuration.Initialize(PluginInterface);

			// you might normally want to embed resources and load them from the manifest stream
			var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
			var goatImage = PluginInterface.UiBuilder.LoadImage(imagePath);
			var pokerGame = new PokerGame(PartyList, ClientState);

			ConfigWindow = new ConfigWindow(this);
			PokerWindow = new PokerWindow(pokerGame, ChatGui);
			MainWindow = new MainWindow(this, goatImage);

			Logger.Log = pluginLog;

			WindowSystem.AddWindow(ConfigWindow);
			WindowSystem.AddWindow(PokerWindow);
			WindowSystem.AddWindow(MainWindow);

			_ = CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
			{
				HelpMessage = "Game collection to be used in the Dragon's Rest"
			});

			PluginInterface.UiBuilder.Draw += DrawUI;
			PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
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
			PokerWindow.IsOpen = true;
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
