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
        public decimal stack = 50000.0M;
        public decimal bet;
        public List<Card> handCards = new List<Card>();
        public HandType handType;
        public int handValue;

        public bool isAI = false;

        public Action ActionPrompt(decimal targetBet)
        {
            if (isAI)
            {
                return Action.Check;
            }

            if (targetBet > bet)
            {
                Console.WriteLine("(C)all / (R)aise / (F)old:");
            }
            else
            {
                Console.WriteLine("(C)heck / (B)et / (F)old:");
            }
            string input = Console.ReadLine();

            if (input == "B" || input == "b")
            {
                return Action.Raise;
            }
            if (input == "F" || input == "f")
            {
                return Action.Fold;
            }
            return Action.Check;
        }

        public bool AddBet(decimal amount)
        {
            if (stack < amount)
            {
                return false;
            }

            stack -= amount;
            bet += amount;
            
            return true;
        }
    }

    public enum Action
    {
        Check,
        Raise,
        Fold
    }
}