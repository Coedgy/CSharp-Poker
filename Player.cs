using System;
using System.Collections.Generic;

namespace CSharp_Poker
{
    public class Player
    {
        public int ID;
        public decimal balance = 0.0M;
        public string name;
        public decimal stack = 2000.0M;
        public decimal bet;
        public List<Card> handCards = new List<Card>();
        public HandType handType;
        public int handValue;

        public bool folded = false;

        public bool isAI = false;

        public Action ActionPrompt(decimal targetBet)
        {
            if (isAI)
            {
                return Action.Check;
            }

            if (targetBet - bet >= stack)
            {
                Console.WriteLine("(A)ll in / (F)old:");
            }
            else if (targetBet > bet)
            {
                Console.WriteLine("(C)all / (R)aise / (F)old:");
            }
            else
            {
                Console.WriteLine("(C)heck / (B)et / (F)old:");
            }
            string input = Console.ReadLine();

            if (input == "B" || input == "b" || input == "R" || input == "r")
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

        public void WinMoney(decimal amount)
        {
            stack += amount;
        }

        public void ResetBet()
        {
            bet = 0.0M;
        }
    }

    public enum Action
    {
        Check,
        Raise,
        Fold
    }
}