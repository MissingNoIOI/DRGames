using Dalamud.Game.Chat;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DRGames.Poker
{
	public static partial class PokerChatDetection
	{
		public static void Apply(IHandleableChatMessage chatMessage, PokerGame game)
		{
			var senderName = chatMessage.Sender.TextValue.Trim();
			if (string.IsNullOrWhiteSpace(senderName))
			{
				senderName = chatMessage.OriginalSender.ToString().Trim();
			}

			var player = game.PlayerList.FirstOrDefault(candidate => candidate.IsPlaying && IsPlayerSender(candidate, senderName));
			if (player is null || !TryParseAction(chatMessage.Message.TextValue, out var action))
			{
				return;
			}

			game.TryApplyDetectedAction(player, action);
		}

		private static bool IsPlayerSender(Player player, string senderName)
		{
			var senderWithoutWorld = senderName.Split('@')[0].Trim();
			return senderWithoutWorld.Equals(player.Name, StringComparison.OrdinalIgnoreCase)
				|| senderWithoutWorld.Contains(player.Name, StringComparison.OrdinalIgnoreCase);
		}

		private static bool TryParseAction(string message, out PokerAction action)
		{
			if (FoldRegex().IsMatch(message))
			{
				action = PokerAction.Fold;
				return true;
			}
			if (CheckRegex().IsMatch(message))
			{
				action = PokerAction.Check;
				return true;
			}
			if (RaiseRegex().IsMatch(message))
			{
				action = PokerAction.Raise;
				return true;
			}
			if (OpenRegex().IsMatch(message))
			{
				action = PokerAction.Open;
				return true;
			}
			if (CallRegex().IsMatch(message))
			{
				action = PokerAction.Call;
				return true;
			}

			action = PokerAction.None;
			return false;
		}

		[GeneratedRegex(@"\b(fold|folding|folded)\b", RegexOptions.IgnoreCase)]
		private static partial Regex FoldRegex();

		[GeneratedRegex(@"\b(check|checking|checked)\b", RegexOptions.IgnoreCase)]
		private static partial Regex CheckRegex();

		[GeneratedRegex(@"\b(raise|raising|raised)\b", RegexOptions.IgnoreCase)]
		private static partial Regex RaiseRegex();

		[GeneratedRegex(@"\b(open|opening|opened)\b", RegexOptions.IgnoreCase)]
		private static partial Regex OpenRegex();

		[GeneratedRegex(@"\b(call|calling|called)\b", RegexOptions.IgnoreCase)]
		private static partial Regex CallRegex();
	}
}
