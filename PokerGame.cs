using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PokeriPeli
{
    class Program
    {
        public static List<PokerTable> tableList;

        static void Main(string[] args)
        {
            System.Console.Clear();
            //System.Console.BackgroundColor = System.ConsoleColor.DarkBlue;

            tableList = new List<PokerTable>();
            Cards.InitializeList();
            CreateTestTable();
        }

        static void CreateTestTable()
        {
            bool boolean = false; 
            do
            {
                PokerTable testTable = new PokerTable();
                testTable.tableID = 1;
                testTable.smallBlind = 5.00M;
                testTable.bigBlind = 10.00M;
                testTable.InitializeTable();

                for (int i = 0; i < 10; i++)
                {
                    Player player = new Player();
                    player.name = "Player " + (i + 1);
                    player.ID = i + 1;
                    testTable.AddPlayer(player);
                }

                testTable.ShuffleDeck();
            
                System.Console.WriteLine(" - ");
                testTable.DealCards();

                testTable.CalculateHands();
            
                foreach (Player player in testTable.players)
                {
                    System.Console.WriteLine(player.name + ": " + player.handType);
                    FormatCardList(player.handCards);
                    System.Console.WriteLine("");
                }

                System.Console.WriteLine("Board: ");
                FormatCardList(testTable.board);
                System.Console.WriteLine("");
            
                testTable.GetWinners();

                foreach (var player in testTable.players)
                {
                    if (player.handType == HandType.ThreeOfAKind)
                    {
                        boolean = true;
                    }
                }

                testTable.ClearCards();
            } while (boolean == false);
        }

        public static void FormatCardList(List<Card> list)
        {
            foreach (Card card in list)
            {
                System.Console.Write(" | ");
                string color;
                if (card.color == Color.clubs)
                {
                    color = "♣";
                    System.Console.ForegroundColor = ConsoleColor.White;
                }else if (card.color == Color.spades)
                {
                    color = "♠";
                    System.Console.ForegroundColor = ConsoleColor.White;
                }else if (card.color == Color.diamonds)
                {
                    color = "♦";
                    System.Console.ForegroundColor = ConsoleColor.Red;
                }else
                {
                    color = "♥";
                    System.Console.ForegroundColor = ConsoleColor.Red;
                }

                string value;
                if (card.value == 11)
                {
                    value = "J";
                }else if (card.value == 12)
                {
                    value = "Q";
                }else if (card.value == 13)
                {
                    value = "K";
                }else if (card.value == 1)
                {
                    value = "A";
                }else
                {
                    value = card.value.ToString();
                }
                System.Console.Write(color);
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine(" " + value);
            }
        }
    }

    public class PokerTable
    {
        public int tableID;
        public List<Card> deck = new List<Card>();
        public List<Player> players = new List<Player>();
        public int pot;
        public decimal smallBlind;
        public decimal bigBlind;
        public List<Seat> seats = new List<Seat>();
        public int maxPlayers = 10;
        public List<Card> board = new List<Card>();

        Random r = new Random();

        public void InitializeTable()
        {
            for (int i = 1; i < maxPlayers + 1; i++)
            {
                seats.Add(new Seat());
            }
            NewDeck();
        }

        public void NewDeck()
        {
            deck.Clear();
            deck = new List<Card>(Cards.list);
        }

        public void ShuffleDeck()
        {
            deck = deck.OrderBy(i => r.Next()).ToList();
        }

        public void AddPlayer(Player player)
        {
            if (players.Count < maxPlayers)
            {
                players.Add(player);
                bool seatFound = false;
                int i = 0;
                do
                {
                    if (seats[i].isEmpty())
                    {
                        seats[i].player = player;
                        seatFound = true;
                    }
                    i++;
                } while (seatFound == false);
            }
            else
            {
                throw new Exception("Maximum number of players exceeded");
            }
        }

        public void DealCards()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (Seat seat in seats)
                {
                    if (!seat.isEmpty())
                    {
                        seat.player.handCards.Add(deck[0]);
                        deck.RemoveAt(0);
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                board.Add(deck[0]);
                deck.RemoveAt(0);
            }
        }

        public void ClearCards()
        {
            foreach (Seat seat in seats)
            {
                if (!seat.isEmpty())
                {
                    deck.Add(seat.player.handCards[0]);
                    seat.player.handCards.RemoveAt(0);
                    deck.Add(seat.player.handCards[0]);
                    seat.player.handCards.RemoveAt(0);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                deck.Add(board[0]);
                board.RemoveAt(0);
            }
        }

        public void CalculateHands()
        {
            foreach (var player in players)
            {
                Tuple<HandType, int> bestHandValues = PokerGame.GetBestHand(player.handCards, board);
                player.handType = bestHandValues.Item1;
                player.handValue = bestHandValues.Item2;
            }
        }

        public void GetWinners()
        {
            List<Player> winners = new List<Player>();
            int biggestValue = 0;
            HandType biggestType = HandType.High;
            
            foreach (var player in players)
            {
                if (player.handType > biggestType)
                {
                    biggestType = player.handType;
                    biggestValue = player.handValue;
                }
                else if (player.handType == biggestType)
                {
                    if (player.handValue > biggestValue)
                    {
                        biggestType = player.handType;
                        biggestValue = player.handValue;
                    }
                }
            }

            foreach (var player in players)
            {
                if (player.handType == biggestType && player.handValue == biggestValue)
                {
                    winners.Add(player);
                    Console.WriteLine(player.name);
                }
            }
        }
    }

    public class PokerGame
    {
        public static Tuple<HandType, int> GetBestHand(List<Card> hand, List<Card> board)
        {
            List<Card> cards = new List<Card>();
            board.ForEach(x => cards.Add(x));
            hand.ForEach(x => cards.Add(x));
            cards.FindAll(x => x.value == 1).ToList().ForEach(x => cards.Add(new Card(){color = x.color, ID = x.ID, value = 14}));

            cards = cards.OrderBy(x => x.value).ToList();

            // Straight flush
            int chain = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];

                if (i == cards.Count - 1)
                {
                    if (chain >= 4 && card.color == cards[i-1].color)
                    {
                        return new Tuple<HandType, int>(HandType.StraightFlush, card.value + card.value - 1 + card.value - 2 + card.value - 3 + card.value - 4);
                    }
                    break;
                }
                
                if (card.value + 1 == cards[i + 1].value && card.color == cards[i+1].color)
                {
                    chain++;
                    //Console.WriteLine(card.value + " " + card.color + " " + cards[i + 1].value +  " " + cards[i+1].color);
                }
                else if (card.value == cards[i + 1].value)
                {
                    
                }
                else
                {
                    if (chain >= 4)
                    {
                        return new Tuple<HandType, int>(HandType.StraightFlush, card.value + card.value - 1 + card.value - 2 + card.value - 3 + card.value - 4);
                    }
                    chain = 0;
                }
            }
            
            // Four of a kind
            chain = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];

                if (i == cards.Count - 1)
                {
                    if (chain == 3)
                    {
                        return new Tuple<HandType, int>(HandType.FourOfAKind, card.value * 4000 + cards[cards.Count-5].value);
                    }
                    break;
                }
                
                //Console.WriteLine(card.value + " " + card.color + " " + cards[i + 1].value +  " " + cards[i+1].color);
                
                if (card.value == cards[i + 1].value)
                {
                    chain++;
                }
                else
                {
                    if (chain == 3)
                    {
                        if (card.value == 1)
                        {
                            return new Tuple<HandType, int>(HandType.FourOfAKind, card.value * 40000 + cards[cards.Count-1].value);
                        }
                        return new Tuple<HandType, int>(HandType.FourOfAKind, card.value * 4000 + cards[cards.Count-1].value);
                    }
                    chain = 0;
                }
            }
            
            //Full house
            //TODO: Count three of a kind and pair
            
            //Flush
            //TODO: Same as straight checking but chain raises if is same color
            
            // Straight
            chain = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];

                if (i == cards.Count - 1)
                {
                    if (chain >= 4)
                    {
                        return new Tuple<HandType, int>(HandType.Straight, card.value + card.value - 1 + card.value - 2 + card.value - 3 + card.value - 4);
                    }
                    break;
                }
                
                if (card.value + 1 == cards[i + 1].value)
                {
                    chain++;
                }
                else if (card.value == cards[i + 1].value)
                {
                    
                }
                else
                {
                    if (chain >= 4)
                    {
                        return new Tuple<HandType, int>(HandType.Straight, card.value + card.value - 1 + card.value - 2 + card.value - 3 + card.value - 4);
                    }
                    chain = 0;
                }
            }
            
            // Three of a kind
            chain = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];

                if (i == cards.Count - 1)
                {
                    if (chain == 2)
                    {
                        Console.WriteLine(card.value * 3000 + cards[cards.Count-4].value + cards[cards.Count-5].value);
                        return new Tuple<HandType, int>(HandType.ThreeOfAKind, card.value * 3000 + cards[cards.Count-3].value + cards[cards.Count-4].value);
                    }
                    break;
                }
                
                //Console.WriteLine(card.value + " " + card.color + " " + cards[i + 1].value +  " " + cards[i+1].color);
                
                if (card.value == cards[i + 1].value)
                {
                    chain++;
                }
                else
                {
                    if (chain == 2)
                    {
                        if (card.value == 1)
                        {
                            return new Tuple<HandType, int>(HandType.ThreeOfAKind, card.value * 30000 + cards[cards.Count-1].value + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1]).value);
                        }
                        return new Tuple<HandType, int>(HandType.ThreeOfAKind, card.value * 3000 + cards[cards.Count-1].value + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1]).value);
                    }
                    chain = 0;
                }
            }
            
            // Two pairs
            //TODO: Start counting all pairs found
            
            // Pair
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];

                if (i == cards.Count - 1)
                {
                    if (chain == 1)
                    {
                        return new Tuple<HandType, int>(HandType.Pair, card.value * 2000 + cards[cards.Count-2].value + cards[cards.Count-3].value + cards[cards.Count-4].value);
                    }
                    break;
                }

                if (card.value == cards[i + 1].value)
                {
                    chain++;
                }
                else
                {
                    if (chain == 1)
                    {
                        if (card.value == 1)
                        {
                            return new Tuple<HandType, int>(HandType.Pair, card.value * 20000 + cards[cards.Count-1].value + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1]).value);
                        }
                        return new Tuple<HandType, int>(HandType.Pair, card.value * 2000 + cards[cards.Count-1].value + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1]).value);
                    }
                    chain = 0;
                }
            }
            
            // High
            //TODO: Fix value
            return new Tuple<HandType, int>(HandType.High, cards[cards.Count - 1].value + cards[cards.Count - 2].value + cards[cards.Count - 3].value + cards[cards.Count - 4].value + cards[cards.Count - 5].value);
        }
    }

    public class Seat 
    {
        public Player player;

        public bool isEmpty()
        {
            if (player == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static class Cards
    {
        public static List<Card> list;
        public static void InitializeList()
        {
            list = new List<Card>();
            int id = 1;
            for (int i = 1; i < 5; i++)
            {
                for (int x = 1; x < 14; x++)
                {
                    Card card = new Card();
                    card.ID = id;
                    card.color = (Color)i;
                    card.value = x;
                    list.Add(card);
                    id++;
                }
            }
        }
    }

    public class Card
    {
        public int ID;
        public int value;
        public Color color;
    }

    public enum Color
    {
        clubs = 1,
        spades = 2,
        diamonds = 3,
        hearts = 4
    }

    public enum TableStage
    {
        preflop,
        flop,
        turn,
        river
    }

    public enum HandType
    {
        High,
        Pair,
        TwoPairs,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }
}