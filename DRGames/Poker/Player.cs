using System;

namespace DRGames.Poker
{
    public class Player
    {
        public string Name { get; set; }
        public Tuple<Card, Card>? Hand { get; set; }
        public int Chips { get; set; } = 0;

        public Player(string name)
        {
            Name = name;
        }
    }
}
