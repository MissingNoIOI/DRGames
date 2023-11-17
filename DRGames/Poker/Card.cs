namespace DRGames.Poker
{
    public record Card(Rank Rank, Suit Suite)
    {
        public string FullName => $"{Suite.Sign} {Rank.Name} of {Suite.Name} {Suite.Sign}";
    }
}
