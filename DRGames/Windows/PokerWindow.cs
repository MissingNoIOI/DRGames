using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Game.Chat;
using DRGames.Games;
using DRGames.Poker;
using FFXIVClientStructs.FFXIV.Common.Math;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using DRGames.Poker.Deck;

namespace DRGames.Windows
{
	public class PokerWindow : Window, IDisposable
	{
		private readonly PokerGame game;
		private readonly IChatGui chatGui;
		private readonly IGameChat gameChat;
		private readonly CommonGamePanel commonGamePanel;
		private bool resultsAnnounced;
		private bool chatDetectionEnabled;

		public PokerWindow(PokerGame game, IChatGui chatGui, IGameChat gameChat) : base("DRGames Poker", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse, false)
		{
			Size = new Vector2(0, 0);
			SizeCondition = ImGuiCond.Always;

			this.game = game;
			this.chatGui = chatGui;
			this.gameChat = gameChat;
			commonGamePanel = new CommonGamePanel(game);
			chatGui.ChatMessage += OnChatMessage;
		}

		public void Dispose()
		{
			chatGui.ChatMessage -= OnChatMessage;
			commonGamePanel.Dispose();
		}

		private void DrawPlayerDetails(IPlayer player)
		{
			var pokerPlayer = (Player)player;
			var selectedAction = (int)pokerPlayer.Action;
			ImGui.BeginDisabled(pokerPlayer.Hand is null);
			var currentActionIsValid = game.IsActionValid(pokerPlayer, pokerPlayer.Action);
			if (!currentActionIsValid)
			{
				ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.55f, 0.55f, 0.55f, 1f));
			}
			ImGui.Text("Action");
			ImGui.SameLine();
			var comboOpen = ImGui.BeginCombo("##PokerAction", pokerPlayer.Action.ToString());
			if (!currentActionIsValid)
			{
				ImGui.PopStyleColor();
			}
			if (comboOpen)
			{
				foreach (var action in Enum.GetValues<PokerAction>())
				{
					var isSelected = action == pokerPlayer.Action;
					if (!game.IsActionValid(pokerPlayer, action))
					{
						ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.55f, 0.55f, 0.55f, 1f));
					}
					if (ImGui.Selectable(action.ToString(), isSelected))
					{
						selectedAction = (int)action;
					}
					if (!game.IsActionValid(pokerPlayer, action))
					{
						ImGui.PopStyleColor();
					}
					if (isSelected)
					{
						ImGui.SetItemDefaultFocus();
					}
				}
				ImGui.EndCombo();
			}
			game.SetAction(pokerPlayer, (PokerAction)selectedAction);
			ImGui.EndDisabled();

			if (pokerPlayer.Hand != null)
			{
				ImGui.Text("Current Hand");
				ImGui.SameLine();
				ImGui.Text(string.Join(" | ", SortCards(new[] { pokerPlayer.Hand.Item1, pokerPlayer.Hand.Item2 })));
			}
			else if (ImGui.Button("Deal Hand"))
			{
				game.DealHand(pokerPlayer);
				var hand = SortCards(new[] { pokerPlayer.Hand.Item1, pokerPlayer.Hand.Item2 });
				gameChat.SendTell(
					pokerPlayer.Name,
					pokerPlayer.World,
					$"Your cards are {hand.First().FullName} and {hand.Last().FullName}");
			}
		}

		private void OnChatMessage(IHandleableChatMessage chatMessage)
		{
			if (chatDetectionEnabled && game.Stage != 3)
			{
				PokerChatDetection.Apply(chatMessage, game);
			}
		}

		private static IEnumerable<Card> SortCards(IEnumerable<Card> cards)
		{
			return cards
				.OrderBy(card => card.Rank.Value)
				.ThenBy(card => card.Suite?.Name);
		}

		private static string FormatHand(Tuple<Card, Card> hand)
		{
			return string.Join(" and ", SortCards(new[] { hand.Item1, hand.Item2 }));
		}

		private void AnnounceResults()
		{
			var hands = string.Join(", ", game.PlayerList
				.Where(player => player.IsPlaying && player.Hand is not null)
				.Select(player => $"{player.Name}: {FormatHand(player.Hand!)}"));
			var winners = string.Join(", ", game.Winners
				.Select(winner => $"{winner.User!.Name} with a {winner.RankName}"));

			gameChat.SendPartyMessage($"Player hands: {hands}");
			gameChat.SendPartyMessage($"Winners: {winners}");
		}

		public override void Draw()
		{
			if (!commonGamePanel.DrawPlayers(DrawPlayerDetails))
			{
				return;
			}
			ImGui.Text($"Total Betting Pool: {game.PlayerList.Sum(x => x.Bet):N0} gil");
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
							var newCard = game.CommunityCards.Last();
							var cards = string.Join(" | ", SortCards(game.CommunityCards));
							if (game.Stage < 2)
							{
								gameChat.SendPartyMessage(
									$"The game is now in the {Helpers.TranslateInt(game.Stage)} stage and the community cards are {cards}");
							}
							else
							{
								gameChat.SendPartyMessage(
									$"The game is now in the {Helpers.TranslateInt(game.Stage)} stage, the new card is {newCard}, so the community cards are {cards}");
							}
							chatGui.Print($"Copied the community cards to the clipboard");
						}
					}
				}

				if (game.CommunityCards.Count > 0)
				{
					ImGui.Spacing();
					ImGui.Text("Community Cards");
					ImGui.Text(string.Join(" | ", SortCards(game.CommunityCards)));
				}

				ImGui.Spacing();
				ImGui.Separator();
			}


			if (game.Stage == 3)
			{
				if (!resultsAnnounced)
				{
					AnnounceResults();
					resultsAnnounced = true;
				}

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
				resultsAnnounced = false;
			}

			ImGui.SameLine();
			ImGui.Checkbox("Chat detection", ref chatDetectionEnabled);
		}
	}
}
