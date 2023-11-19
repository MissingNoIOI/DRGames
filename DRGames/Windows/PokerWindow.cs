using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using DRGames.Poker;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using System;
using System.Linq;

namespace DRGames.Windows
{
	public class PokerWindow : Window, IDisposable
	{
		private PokerGame Game { get; init; }
		private IChatGui ChatGui { get; init; }
		public PokerWindow(PokerGame game, IChatGui chatGui) : base("DRGames Poker", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse, false)
		{
			Size = new Vector2(0, 0);
			SizeCondition = ImGuiCond.Always;

			Game = game;
			ChatGui = chatGui;
		}

		public void Dispose()
		{
		}

		[Obsolete]
		public override void Draw()
		{
			Game.Update();

			// Current players
			ImGui.Text($"Current Players ");
			ImGui.Spacing();
			ImGui.Separator();
			var id = 0;
			foreach (var player in Game.PlayerList)
			{
				ImGui.PushID(id);
				ImGui.Text("Player: " + player.Name);
				var tmp = player.IsPlaying;
				if (ImGui.Checkbox("Is Playing ", ref tmp))
				{
					player.IsPlaying = tmp;
				}
				if (player.IsPlaying)
				{
					if (player.Hand != null)
					{
						ImGui.Text("Current Hand");
						ImGui.Text(player.Hand.Item1.FullName);
						ImGui.Text(player.Hand.Item2.FullName);
					}
					else
					{
						if (ImGui.Button("Deal Hand"))
						{
							Game.DealHand(player);
							ImGui.SetClipboardText($"/tell {player.Name}@{player.World} Your cards are {player.Hand.Item1.FullName} and {player.Hand.Item2.FullName}");
							ChatGui.Print($"Copied {player.Name}'s hand to the clipboard");
						}
					}
				}
				ImGui.Spacing();
				ImGui.Separator();
				ImGui.PopID();
				id++;
			}

			//Community Cards
			ImGui.Dummy(new Vector2(0, 20));
			ImGui.Text("Current Game");
			ImGui.Text($"{Helpers.TranslateInt(Game.Stage)} stage");
			ImGui.Spacing();
			if (Game.Stage != 3)
			{
				if (ImGui.Button("Next Stage"))
				{
					Game.NextStage();
					var cards = string.Join(" | ", Game.CommunityCards);
					if (Game.Stage < 2)
					{
						ImGui.SetClipboardText($"/party The game is now in the {Helpers.TranslateInt(Game.Stage)} stage and the community cards are {cards}");
					}
					else
					{
						ImGui.SetClipboardText($"/party The game is now in the {Helpers.TranslateInt(Game.Stage)} stage, the new card is {Game.CommunityCards.Last()}, so the community cards are {cards}");
					}
					ChatGui.Print($"Copied the community cards to the clipboard");
				}
			}

			if (Game.CommunityCards.Count > 0)
			{
				ImGui.Spacing();
				ImGui.Text("Community Cards");
				foreach (var card in Game.CommunityCards)
				{
					ImGui.Text(card.FullName);
				}
			}

			ImGui.Spacing();
			ImGui.Separator();

			// End game button
			ImGui.Dummy(new Vector2(0, 20));

			if (ImGui.Button("End Current Game"))
			{
				Game.EndGame();
			}

		}
	}
}
