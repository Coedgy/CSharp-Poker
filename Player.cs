using System;
using System.Linq;
using System.Collections.Generic;

namespace PokeriPeli
{
    public class Player
    {
        public int ID;
        public decimal balance;
        public string name;
        public decimal stack;
        public decimal bet;
        public List<Card> handCards = new List<Card>();
        public HandType handType;
        public int handValue;
    }
}