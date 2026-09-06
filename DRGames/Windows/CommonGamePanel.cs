using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using DRGames.Games;
using System;
using System.Text.RegularExpressions;

namespace DRGames.Windows
{
	public sealed partial class CommonGamePanel : IDisposable
	{
		private readonly IGame game;
		private readonly IChatGui chatGui;
		private const string TradeCompleteMessage = "Trade complete.";
		private const string TradeCanceledMessage = "Trade canceled.";

		private TradeState tradeState = TradeState.Idle;
		private string? pendingTradePlayer;
		private long? pendingTradeAmount;

		private enum TradeState
		{
			Idle,
			Requested,
			AwaitingConfirmation,
			AmountReceived
		}

		[GeneratedRegex(@"^(?<player>.+?) wishes to trade with you\.$", RegexOptions.CultureInvariant)]
		public static partial Regex TradeRequest();

		[GeneratedRegex(@"^Trade request sent to (?<player>.+?)\.$", RegexOptions.CultureInvariant)]
		public static partial Regex TradeRequestSent();

		[GeneratedRegex(@"^Awaiting trade confirmation from (?<player>.+?)\.\.\.$", RegexOptions.CultureInvariant)]
		public static partial Regex TradeAwaitingConfirmation();

		[GeneratedRegex(@"^You receive (?<amount>[\d,]+) gil\.$", RegexOptions.CultureInvariant)]
		public static partial Regex TradeGilReceived();

		public CommonGamePanel(IGame game, IChatGui chatGui)
		{
			this.game = game;
			this.chatGui = chatGui;
			this.chatGui.ChatMessage += OnChatMessage;
		}

		public void Dispose()
		{
			chatGui.ChatMessage -= OnChatMessage;
		}

		public bool DrawPlayers(Action<IPlayer> drawPlayerDetails)
		{
			game.Update();

			if (game.Players.Count == 0)
			{
				ImGui.Text("Please invite players to the party");
				return false;
			}

			ImGui.Text("Current Players");
			ImGui.Spacing();
			ImGui.Separator();

			var id = 0;
			foreach (var player in game.Players)
			{
				ImGui.PushID(id++);
				ImGui.Text("Player: " + player.Name);
				ImGui.Text($"Bet: {player.Bet:N0} gil");

				var isPlaying = player.IsPlaying;
				if (ImGui.Checkbox("Is Playing ", ref isPlaying))
				{
					player.IsPlaying = isPlaying;
				}

				if (player.IsPlaying)
				{
					drawPlayerDetails(player);
				}

				ImGui.Spacing();
				ImGui.Separator();
				ImGui.PopID();
			}

			return true;
		}

		private void OnChatMessage(IHandleableChatMessage message)
		{
			if (message.LogKind != XivChatType.SystemMessage)
			{
				return;
			}

			ProcessTradeMessage(message.Message.ToString());
		}

		private void ProcessTradeMessage(string text)
		{
			var requestMatch = TradeRequest().Match(text);
			if (requestMatch.Success)
			{
				StartTrade(requestMatch.Groups["player"].Value);
				return;
			}

			var requestSentMatch = TradeRequestSent().Match(text);
			if (requestSentMatch.Success)
			{
				StartTrade(requestSentMatch.Groups["player"].Value);
				return;
			}

			var awaitingMatch = TradeAwaitingConfirmation().Match(text);
			if (awaitingMatch.Success && tradeState == TradeState.Requested && pendingTradePlayer == awaitingMatch.Groups["player"].Value)
			{
				tradeState = TradeState.AwaitingConfirmation;
				return;
			}

			var gilMatch = TradeGilReceived().Match(text);
			if (gilMatch.Success && tradeState == TradeState.AwaitingConfirmation && long.TryParse(gilMatch.Groups["amount"].Value.Replace(",", string.Empty), out var amount) && amount > 0)
			{
				pendingTradeAmount = amount;
				tradeState = TradeState.AmountReceived;
				return;
			}

			if (text == TradeCanceledMessage)
			{
				ResetTrade();
				return;
			}

			if (text == TradeCompleteMessage)
			{
				if (tradeState == TradeState.AmountReceived && pendingTradePlayer != null && pendingTradeAmount.HasValue)
				{
					game.RecordBet(pendingTradePlayer, pendingTradeAmount.Value);
				}

				ResetTrade();
			}
		}

		private void StartTrade(string playerName)
		{
			pendingTradePlayer = playerName;
			pendingTradeAmount = null;
			tradeState = TradeState.Requested;
		}

		private void ResetTrade()
		{
			pendingTradePlayer = null;
			pendingTradeAmount = null;
			tradeState = TradeState.Idle;
			}
	}
}
