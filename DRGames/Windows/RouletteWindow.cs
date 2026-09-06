using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Chat;
using Dalamud.Plugin.Services;
using DRGames.Games;
using DRGames.Roulette;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace DRGames.Windows
{
	public sealed class RouletteWindow : Window, IDisposable
	{
		private readonly RouletteGame game;
		private readonly IGameChat gameChat;
		private readonly IChatGui chatGui;
		private readonly CommonGamePanel commonGamePanel;
		private bool awaitingResult;

		private static readonly Regex RouletteResultRegex = new(@"Random!\s*\(1-37\)\s*(?<result>\d+)\.?", RegexOptions.Compiled);

		public RouletteWindow(RouletteGame game, IGameChat gameChat, IChatGui chatGui) : base("DRGames Roulette", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse, false)
		{
			Size = new Vector2(0, 0);
			SizeCondition = ImGuiCond.Always;
			this.game = game;
			this.gameChat = gameChat;
			this.chatGui = chatGui;
			commonGamePanel = new CommonGamePanel(game);
			chatGui.ChatMessage += OnChatMessage;
		}

		public void Dispose()
		{
			chatGui.ChatMessage -= OnChatMessage;
			commonGamePanel.Dispose();
		}

		public override void Draw()
		{
			DrawBettingOptions();
			if (!commonGamePanel.DrawPlayers(DrawPlayerDetails, DrawPlayerStatus))
			{
				return;
			}

			ImGui.Spacing();
			if (!game.HasResult)
			{
				if (awaitingResult)
				{
					ImGui.Text("Waiting for the roulette result...");
				}
				else if (ImGui.Button("Roll Roulette"))
				{
					awaitingResult = true;
					gameChat.SendCommand("/dice party 37");
				}
			}
			else
			{
				ImGui.Text($"Roulette result: {(game.Result == 37 ? 0 : game.Result)}");
				if (ImGui.Button("End Game"))
				{
					game.EndGame();
					awaitingResult = false;
				}
			}
		}

		private void DrawBettingOptions()
		{
			ImGui.Text("Roulette Bets");
			ImGui.Text("Single number: 35:1 | Even/Odd: 1:1 | Low/High: 1:1 | Dozen: 2:1 | Street: 11:1 | Double street: 5:1");
			ImGui.Separator();
		}

		private void DrawPlayerDetails(IPlayer player)
		{
			var roulettePlayer = (RoulettePlayer)player;
			roulettePlayer.RouletteBet ??= new RouletteBet
			{
				Type = RouletteBetType.SingleNumber,
				Value = 1
			};

			var bet = roulettePlayer.RouletteBet;
			ImGui.BeginDisabled(awaitingResult || game.HasResult);
			var selectedType = (int)bet.Type;
			if (ImGui.BeginCombo("Bet Type", GetBetTypeLabel(bet.Type)))
			{
				foreach (var type in Enum.GetValues<RouletteBetType>())
				{
					var isSelected = type == bet.Type;
					if (ImGui.Selectable(GetBetTypeLabel(type), isSelected))
					{
						selectedType = (int)type;
					}
					if (isSelected)
					{
						ImGui.SetItemDefaultFocus();
					}
				}
				ImGui.EndCombo();
			}

			if ((RouletteBetType)selectedType != bet.Type)
			{
				bet.Type = (RouletteBetType)selectedType;
				bet.Value = GetBetValues(bet.Type).FirstOrDefault();
			}

			var values = GetBetValues(bet.Type);
			if (values.Count > 0)
			{
				var selectedValue = values.IndexOf(bet.Value);
				if (selectedValue < 0)
				{
					selectedValue = 0;
					bet.Value = values[0];
				}

				if (ImGui.BeginCombo("Bet Value", GetBetValueLabel(bet.Type, bet.Value)))
				{
					foreach (var value in values)
					{
						var isSelected = value == bet.Value;
						if (ImGui.Selectable(GetBetValueLabel(bet.Type, value), isSelected))
						{
							bet.Value = value;
						}
						if (isSelected)
						{
							ImGui.SetItemDefaultFocus();
						}
					}
					ImGui.EndCombo();
				}
			}
			ImGui.EndDisabled();
		}

		private void DrawPlayerStatus(IPlayer player)
		{
			var roulettePlayer = (RoulettePlayer)player;
			if (roulettePlayer.Won is null)
			{
				return;
			}

			ImGui.PushStyleColor(ImGuiCol.Text, roulettePlayer.Won == true
				? new Vector4(0.2f, 1f, 0.2f, 1f)
				: new Vector4(1f, 0.2f, 0.2f, 1f));
			ImGui.Text(roulettePlayer.Won == true
				? $"Won: {roulettePlayer.Payout:N0} gil"
				: "Lost");
			ImGui.PopStyleColor();
		}

		private void OnChatMessage(IHandleableChatMessage chatMessage)
		{
			var match = RouletteResultRegex.Match(chatMessage.Message.TextValue);
			if (!awaitingResult || !match.Success || !int.TryParse(match.Groups["result"].Value, out var result) || result is < 1 or > 37)
			{
				return;
			}

			game.Evaluate(result);
			awaitingResult = false;
		}

		private static string GetBetTypeLabel(RouletteBetType type)
		{
			return type switch
			{
				RouletteBetType.SingleNumber => "Single number",
				RouletteBetType.DoubleStreet => "Double street",
				_ => type.ToString()
			};
		}

		private static List<int> GetBetValues(RouletteBetType type)
		{
			return type switch
			{
				RouletteBetType.SingleNumber => Enumerable.Range(1, 36).ToList(),
				RouletteBetType.Dozen => new List<int> { 1, 13, 25 },
				RouletteBetType.Street => Enumerable.Range(0, 12).Select(index => index * 3 + 1).ToList(),
				RouletteBetType.DoubleStreet => Enumerable.Range(0, 11).Select(index => index * 3 + 1).ToList(),
				_ => new List<int>()
			};
		}

		private static string GetBetValueLabel(RouletteBetType type, int value)
		{
			return type switch
			{
				RouletteBetType.Dozen => $"{value}-{value + 11}",
				RouletteBetType.Street => $"{value}-{value + 2}",
				RouletteBetType.DoubleStreet => $"{value}-{value + 5}",
				_ => value.ToString()
			};
		}
	}
}
