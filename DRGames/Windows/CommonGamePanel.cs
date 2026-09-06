using Dalamud.Bindings.ImGui;
using DRGames.Games;
using ECommons.GameHelpers;
using System;

namespace DRGames.Windows
{
	public sealed class CommonGamePanel : IDisposable
	{
		private readonly IGame game;

		public CommonGamePanel(IGame game)
		{
			this.game = game;
			TradeDetectionManager.OnTradeEnd += OnTradeEnd;
		}

		public void Dispose()
		{
			TradeDetectionManager.OnTradeEnd -= OnTradeEnd;
		}

		public bool DrawPlayers(Action<IPlayer> drawPlayerDetails, Action<IPlayer>? drawPlayerStatus = null)
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
				ImGui.SameLine();

				var isPlaying = player.IsPlaying;
				if (ImGui.Checkbox("Is Playing ", ref isPlaying))
				{
					player.IsPlaying = isPlaying;
				}

				ImGui.Text($"Bet: {player.Bet:N0} gil");
				drawPlayerStatus?.Invoke(player);

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

		private void OnTradeEnd(Dalamud.Game.ClientState.Objects.SubKinds.IPlayerCharacter counterparty, TradeDetectionManager.TradeDescriptor trade)
		{
			if (trade.ReceivedGil > 0)
			{
				game.RecordBet(counterparty.Name.TextValue, trade.ReceivedGil);
			}
		}
	}
}
