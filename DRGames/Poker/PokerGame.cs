using Dalamud.Plugin.Services;
using DRGames.Poker.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using static DRGames.Helpers;

namespace DRGames.Poker
{
	public class PokerGame
	{
		private IPartyList PartyList { get; init; }
		private IClientState ClientState { get; init; }

		private CardDeck CardDeck { get; init; } = new CardDeck();

		public List<Player> PlayerList { get; init; } = new List<Player>();
		public List<Card> CommunityCards { get; set; } = new List<Card>();
		public int Stage { get; set; } = 0;

		[Obsolete]
		public string partyNames
		{
			get
			{
				var result = "";
				foreach (var member in PartyList)
				{
					if (member.Name.TextValue != ClientState.LocalPlayer?.Name.TextValue)
					{
						result += member.Name;
						result += " ";
					}
				}

				return result;
			}
		}

		public PokerGame(IPartyList partyList, IClientState clientState)
		{
			PartyList = partyList;
			ClientState = clientState;
		}

		public void Update()
		{
			// Add new players in the party to the game
			foreach (var member in PartyList)
			{
				if (member.Name.TextValue == ClientState.LocalPlayer!.Name.TextValue)
				{
					continue;
				}

				if (!PlayerList.Any(x => x.Name == member.Name.TextValue))
				{
					PlayerList.Add(new Player(member.Name.TextValue, member.World.GetWithLanguage(ClientState.ClientLanguage)!.Name));
				}
			}
			// Remove players that have left the party
			var toRemove = PlayerList.Where(x => !PartyList.Any(y => y.Name.TextValue == x.Name)).ToList();
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
			}
			CardDeck.GenerateNewDeck();
			CommunityCards.Clear();
			Stage = 0;
		}

		public void DealHand(Player player)
		{
			player.Hand = Tuple.Create(CardDeck.DrawCard(), CardDeck.DrawCard());
		}

		public void NextStage()
		{
			switch (Stage)
			{
				case 0:
					CommunityCards.Add(CardDeck.DrawCard());
					CommunityCards.Add(CardDeck.DrawCard());
					CommunityCards.Add(CardDeck.DrawCard());
					Stage++;
					break;
				case 1:
					CommunityCards.Add(CardDeck.DrawCard());
					Stage++;
					break;
				case 2:
					CommunityCards.Add(CardDeck.DrawCard());
					Stage++;
					break;
				default:
					Logger.Log.Error("Tried to advance game to an invalid stage");
					break;
			}
		}
	}
}
