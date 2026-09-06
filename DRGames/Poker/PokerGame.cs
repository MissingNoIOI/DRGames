using Dalamud.Plugin.Services;
using DRGames.Games;
using DRGames.Poker.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using static DRGames.Helpers;
using static DRGames.Poker.Solver;

namespace DRGames.Poker
{
	public class PokerGame : IGame
	{
		private readonly IPartyList partyList;
		private readonly IObjectTable objectTable;

		private readonly CardDeck cardDeck = new CardDeck();

		public List<Player> PlayerList { get; init; } = new List<Player>();
		public IReadOnlyList<IPlayer> Players => PlayerList;
		public List<Card> CommunityCards { get; set; } = new List<Card>();
		public int Stage { get; set; } = 0;

		public List<HandRank> Winners { get; set; } = new List<HandRank>();

		[Obsolete]
		public string partyNames
		{
			get
			{
				var result = "";
				foreach (var member in partyList)
				{
					if (member.Name.TextValue != objectTable.LocalPlayer?.Name.TextValue)
					{
						result += member.Name;
						result += " ";
					}
				}

				return result;
			}
		}

		public PokerGame(IPartyList partyList, IObjectTable objectTable)
		{
			this.partyList = partyList;
			this.objectTable = objectTable;
		}

		public void RecordBet(string playerName, long amount)
		{
			var player = PlayerList.FirstOrDefault(x => x.Name == playerName);
			if (player != null && amount > 0)
			{
				player.Bet += amount;
			}
		}

		public void Update()
		{
			// Add new players in the party to the game
			foreach (var member in partyList)
			{
				if (member.Name.TextValue == objectTable.LocalPlayer!.Name.TextValue)
				{
					continue;
				}

				if (!PlayerList.Any(x => x.Name == member.Name.TextValue))
				{
					PlayerList.Add(new Player(member.Name.TextValue, member.World.Value.Name.ToString()));
				}
			}
			// Remove players that have left the party
			var toRemove = PlayerList.Where(x => !partyList.Any(y => y.Name.TextValue == x.Name)).ToList();
			foreach (var member in toRemove)
			{
				_ = PlayerList.Remove(member);
			}

		}

		public void EndGame()
		{
			foreach (var player in PlayerList)
			{
				player.Hand = null;
				player.Bet = 0;
			}
			cardDeck.GenerateNewDeck();
			CommunityCards.Clear();
			Stage = 0;
		}

		public void DealHand(Player player)
		{
			player.Hand = Tuple.Create(cardDeck.DrawCard(), cardDeck.DrawCard());
		}

		public void SolveGame()
		{
			var solver = new Solver { CardsOnTable = CommunityCards, Players = PlayerList };
			Winners = solver.GetWinners();
		}

		public void NextStage()
		{
			switch (Stage)
			{
				case 0:
					CommunityCards.Add(cardDeck.DrawCard());
					CommunityCards.Add(cardDeck.DrawCard());
					CommunityCards.Add(cardDeck.DrawCard());
					Stage++;
					break;
				case 1:
					CommunityCards.Add(cardDeck.DrawCard());
					Stage++;
					break;
				case 2:
					CommunityCards.Add(cardDeck.DrawCard());
					SolveGame();
					Stage++;
					break;
				default:
					Logger.Log.Error("Tried to advance game to an invalid stage");
					break;
			}
		}
	}
}
