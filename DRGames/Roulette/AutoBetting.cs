using Dalamud.Game.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DRGames.Roulette
{
	public static partial class AutoBetting
	{
		public static void Apply(IHandleableChatMessage chatMessage, IEnumerable<RoulettePlayer> players)
		{
			var senderName = chatMessage.Sender.TextValue.Trim();
			if (string.IsNullOrWhiteSpace(senderName))
			{
				senderName = chatMessage.OriginalSender.ToString().Trim();
			}
			var player = players.FirstOrDefault(candidate => candidate.IsPlaying && !candidate.AutoBetCaptured && IsPlayerSender(candidate, senderName));
			var message = chatMessage.Message.TextValue;
			if (string.IsNullOrWhiteSpace(message))
			{
				message = chatMessage.OriginalMessage.ToString();
			}
			if (player is not null && TryParseBet(message, out var bet))
			{
				player.RouletteBet = bet;
				player.AutoBetCaptured = true;
			}
		}

		private static bool IsPlayerSender(RoulettePlayer player, string senderName)
		{
			var senderWithoutWorld = senderName.Split('@')[0].Trim();
			return senderWithoutWorld.Equals(player.Name, StringComparison.OrdinalIgnoreCase)
				|| senderWithoutWorld.Contains(player.Name, StringComparison.OrdinalIgnoreCase);
		}

		private static bool TryParseBet(string message, out RouletteBet bet)
		{
			bet = null!;
			var rangeMatch = BetRangeRegex().Match(message);
			if (rangeMatch.Success && int.TryParse(rangeMatch.Groups["start"].Value, out var start) && int.TryParse(rangeMatch.Groups["end"].Value, out var end))
			{
				if (end - start == 11 && start is 1 or 13 or 25)
				{
					bet = new RouletteBet { Type = RouletteBetType.Dozen, Value = start };
					return true;
				}

				if (end - start == 5 && start % 3 == 1)
				{
					bet = new RouletteBet { Type = RouletteBetType.DoubleStreet, Value = start };
					return true;
				}

				if (end - start == 2 && start % 3 == 1)
				{
					bet = new RouletteBet { Type = RouletteBetType.Street, Value = start };
					return true;
				}
			}

			if (EvenRegex().IsMatch(message))
			{
				bet = new RouletteBet { Type = RouletteBetType.Even };
				return true;
			}
			if (OddRegex().IsMatch(message))
			{
				bet = new RouletteBet { Type = RouletteBetType.Odd };
				return true;
			}
			if (LowRegex().IsMatch(message))
			{
				bet = new RouletteBet { Type = RouletteBetType.Low };
				return true;
			}
			if (HighRegex().IsMatch(message))
			{
				bet = new RouletteBet { Type = RouletteBetType.High };
				return true;
			}

			var dozenMatch = DozenRegex().Match(message);
			if (dozenMatch.Success && int.TryParse(dozenMatch.Groups["value"].Value, out var dozen))
			{
				bet = new RouletteBet { Type = RouletteBetType.Dozen, Value = (dozen - 1) * 12 + 1 };
				return true;
			}

			var numbers = BetNumberRegex().Matches(message);
			if (numbers.Count == 1 && int.TryParse(numbers[0].Groups["value"].Value, out var number) && number is >= 1 and <= 36)
			{
				bet = new RouletteBet { Type = RouletteBetType.SingleNumber, Value = number };
				return true;
			}

			return false;
		}

		[GeneratedRegex(@"(?<!\d)(?<start>\d{1,2})\s*[-–]\s*(?<end>\d{1,2})(?!\d)")]
		private static partial Regex BetRangeRegex();

		[GeneratedRegex(@"(?<!\d)(?<value>\d{1,2})(?!\d)")]
		private static partial Regex BetNumberRegex();

		[GeneratedRegex(@"\b(even)\b", RegexOptions.IgnoreCase)]
		private static partial Regex EvenRegex();

		[GeneratedRegex(@"\b(odd)\b", RegexOptions.IgnoreCase)]
		private static partial Regex OddRegex();

		[GeneratedRegex(@"\b(low|low numbers?)\b", RegexOptions.IgnoreCase)]
		private static partial Regex LowRegex();

		[GeneratedRegex(@"\b(high|high numbers?)\b", RegexOptions.IgnoreCase)]
		private static partial Regex HighRegex();

		[GeneratedRegex(@"\bdozen\s*(?<value>[123])\b", RegexOptions.IgnoreCase)]
		private static partial Regex DozenRegex();
	}
}
