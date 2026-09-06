using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using DRGames.Games;
using DRGames.Poker;
using FFXIVClientStructs.FFXIV.Common.Math;
using Dalamud.Bindings.ImGui;
using System;
using System.Linq;
using Dalamud.Game.Text;

namespace DRGames.Windows
{
	public class PokerWindow : Window, IDisposable
	{
		private readonly PokerGame game;
		private readonly IChatGui chatGui;
		private readonly IGameChat gameChat;
		private readonly CommonGamePanel commonGamePanel;

		public PokerWindow(PokerGame game, IChatGui chatGui, IGameChat gameChat) : base("DRGames Poker", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse, false)
		{
			Size = new Vector2(0, 0);
			SizeCondition = ImGuiCond.Always;

			this.game = game;
			this.chatGui = chatGui;
			this.gameChat = gameChat;
			commonGamePanel = new CommonGamePanel(game, chatGui);
		}

		public void Dispose()
		{
			commonGamePanel.Dispose();
		}

		private void DrawPlayerDetails(IPlayer player)
		{
			var pokerPlayer = (Player)player;
			if (pokerPlayer.Hand != null)
			{
				ImGui.Text("Current Hand");
				ImGui.Text(pokerPlayer.Hand.Item1.FullName);
				ImGui.Text(pokerPlayer.Hand.Item2.FullName);
			}
			else if (ImGui.Button("Deal Hand"))
			{
				game.DealHand(pokerPlayer);
				gameChat.SendTell(
					pokerPlayer.Name,
					pokerPlayer.World,
					$"Your cards are {pokerPlayer.Hand.Item1.FullName} and {pokerPlayer.Hand.Item2.FullName}");
			}
		}

		public override void Draw()
		{
			if (!commonGamePanel.DrawPlayers(DrawPlayerDetails))
			{
				return;
			}
			if (game.PlayerList.Any(x => x.IsPlaying))
			{
				//Community Cards
				ImGui.Dummy(new Vector2(0, 20));
				ImGui.Text("Current Game");
				ImGui.Text($"{Helpers.TranslateInt(game.Stage)} stage");
				ImGui.Spacing();
				if (game.Stage != 3)
				{
					if (game.PlayerList.All(x => x.Hand != null))
					{

						if (ImGui.Button("Next Stage"))
						{
							game.NextStage();
							var cards = string.Join(" | ", game.CommunityCards);
							if (game.Stage < 2)
							{
								gameChat.SendPartyMessage(
									$"The game is now in the {Helpers.TranslateInt(game.Stage)} stage and the community cards are {cards}");
							}
							else
							{
								gameChat.SendPartyMessage(
									$"The game is now in the {Helpers.TranslateInt(game.Stage)} stage, the new card is {game.CommunityCards.Last()}, so the community cards are {cards}");
							}
							chatGui.Print($"Copied the community cards to the clipboard");
						}
					}
				}

				if (game.CommunityCards.Count > 0)
				{
					ImGui.Spacing();
					ImGui.Text("Community Cards");
					foreach (var card in game.CommunityCards)
					{
						ImGui.Text(card.FullName);
					}
				}

				ImGui.Spacing();
				ImGui.Separator();
			}


			if (game.Stage == 3)
			{
				ImGui.Text("Winners: ");
				foreach (var winner in game.Winners)
				{
					ImGui.Text($"{winner.User!.Name} with a {winner.RankName}");
				}
				ImGui.Spacing();
				ImGui.Separator();
			}

			// End game button
			ImGui.Dummy(new Vector2(0, 20));

			if (ImGui.Button("End Current Game"))
			{
				game.EndGame();
			}

		}
	}
}
