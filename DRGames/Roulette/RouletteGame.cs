using Dalamud.Plugin.Services;
using DRGames.Games;
using System.Collections.Generic;
using System.Linq;

namespace DRGames.Roulette
{
	public sealed class RouletteGame : IGame
	{
		private readonly IPartyList partyList;
		private readonly IObjectTable objectTable;

		public List<RoulettePlayer> PlayerList { get; } = new();
		public IReadOnlyList<IPlayer> Players => PlayerList;
		public bool HasResult { get; private set; }
		public int Result { get; private set; }

		public RouletteGame(IPartyList partyList, IObjectTable objectTable)
		{
			this.partyList = partyList;
			this.objectTable = objectTable;
		}

		public void Update()
		{
			foreach (var member in partyList)
			{
				if (member.Name.TextValue == objectTable.LocalPlayer!.Name.TextValue)
				{
					continue;
				}

				if (!PlayerList.Any(player => player.Name == member.Name.TextValue))
				{
					PlayerList.Add(new RoulettePlayer(member.Name.TextValue, member.World.Value.Name.ToString()));
				}
			}

			var toRemove = PlayerList
				.Where(player => !partyList.Any(member => member.Name.TextValue == player.Name))
				.ToList();
			foreach (var player in toRemove)
			{
				PlayerList.Remove(player);
			}
		}

		public void RecordBet(string playerName, long amount)
		{
			var player = PlayerList.FirstOrDefault(candidate => candidate.Name == playerName);
			if (player is not null && amount > 0)
			{
				player.Bet += amount;
			}
		}

		public void Evaluate(int result)
		{
			Result = result;
			HasResult = true;

			foreach (var player in PlayerList)
			{
				player.Won = player.IsPlaying && player.RouletteBet is not null && player.RouletteBet.Wins(result);
				player.Payout = player.Won == true ? player.Bet * player.RouletteBet!.PayoutMultiplier : 0;
			}
		}

		public void EndGame()
		{
			foreach (var player in PlayerList)
			{
				player.Bet = 0;
				player.RouletteBet = null;
				player.Won = null;
				player.Payout = 0;
			}

			HasResult = false;
			Result = 0;
		}
	}
}
