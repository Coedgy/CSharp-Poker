using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PokeriPeli
{
    class Program
    {
        public static StatMode statMode = StatMode.Disabled;
        
        public static int straightFlushCount = 0;
        public static int fourOfAKindCount = 0;
        public static int fullHouseCount = 0;
        public static int flushCount = 0;
        public static int straightCount = 0;
        
        public static List<PokerTable> tableList;

        static void Main(string[] args)
        {
            System.Console.Clear();
            //System.Console.BackgroundColor = System.ConsoleColor.DarkBlue;

            tableList = new List<PokerTable>();
            Cards.InitializeList();
            CIControl();
        }

        static void CIControl()
        {
            int playerCount;

            Console.WriteLine("Gameplay modes:");
            Console.WriteLine("0 - Play-mode");
            Console.WriteLine("1 - Instant game");
            Console.WriteLine("Statistic-modes: (if enabled, 10k games will be played and the player's hands will be logged)");
            Console.WriteLine("2 - Player 1");
            Console.WriteLine("3 - All winners");
            Console.WriteLine("4 - All players");

            var answer = Console.ReadLine();
            int x = 0;
            if (!Int32.TryParse(answer, out x))
            {
                throw new Exception("Invalid input");
            }
            statMode = (StatMode)x;
            
            Console.Clear();
            Console.WriteLine("Player count: ");
            
            answer = Console.ReadLine();
            x = 0;
            if (!Int32.TryParse(answer, out x))
            {
                throw new Exception("Invalid input");
            }
            playerCount = x;

            if (statMode != 0)
            {
                bool run = true;
                while (run)
                {
                    CreateTestTable(playerCount);

                    Console.WriteLine("Play a new game? (Yy/Nn)");
                    string input = Console.ReadLine();
                    if (input == "N" || input == "n")
                    {
                        run = false;
                    }
                    Console.Clear();
                }
            }
            else
            {
                Console.WriteLine("This is not implemented yet..");
                Environment.Exit(0);

                bool run = true;
                while (run)
                {
                    PlayTestGame(playerCount);

                    Console.WriteLine("Play a new game? (Yy/Nn)");
                    string input = Console.ReadLine();
                    if (input == "N" || input == "n")
                    {
                        run = false;
                    }
                    Console.Clear();
                }
            }

            Console.WriteLine("Exiting...");
        }

        static void PlayTestGame(int playerCount)
        { 
            PokerTable testTable = new PokerTable();
            testTable.tableID = 1;
            testTable.smallBlind = 5.00M;
            testTable.bigBlind = 10.00M;
            testTable.InitializeTable();

            for (int i = 0; i < playerCount; i++)
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

            testTable.ClearCards();
        }
        
        static void CreateTestTable(int playerCount)
        {
            //bool boolean = false; 
            int count = 0;
            if (statMode == StatMode.Disabled)
            {
                count = 9999;
            }

            do
            {
                PokerTable testTable = new PokerTable();
                testTable.tableID = 1;
                testTable.smallBlind = 5.00M;
                testTable.bigBlind = 10.00M;
                testTable.InitializeTable();

                for (int i = 0; i < playerCount; i++)
                {
                    Player player = new Player();
                    player.name = "Player " + (i + 1);
                    player.ID = i + 1;
                    testTable.AddPlayer(player);
                }

                testTable.ShuffleDeck();
            
                if (statMode == StatMode.Disabled)
                { 
                    System.Console.WriteLine(" - ");
                }
                testTable.DealCards();

                // testTable.board[0].value = 8;
                // testTable.board[1].value = 8;
                // testTable.board[2].value = 12;
                // testTable.board[3].value = 13;
                // testTable.board[4].value = 1;
                // testTable.players[0].handCards[0].value = 7;
                // testTable.players[0].handCards[1].value = 7;

                testTable.CalculateHands();
                
                if (statMode == StatMode.Disabled)
                { 
                    foreach (Player player in testTable.players)
                    {
                        System.Console.WriteLine(player.name + ": " + player.handType);
                        FormatCardList(player.handCards);
                        System.Console.WriteLine("");
                    }
                     
                    System.Console.WriteLine("Board: "); 
                    FormatCardList(testTable.board); 
                    System.Console.WriteLine("");
                }

                testTable.GetWinners();
                if (statMode == StatMode.Player1)
                {
                    Player targetPlayer = testTable.players[0];
                    if (targetPlayer.handType == HandType.FullHouse)
                    {
                        Program.fullHouseCount++;
                    }else if (targetPlayer.handType == HandType.StraightFlush)
                    {
                        Program.straightFlushCount++;
                    }else if (targetPlayer.handType == HandType.FourOfAKind)
                    {
                        Program.fourOfAKindCount++;
                    }else if (targetPlayer.handType == HandType.Flush)
                    {
                        Program.flushCount++;
                    }else if (targetPlayer.handType == HandType.Straight)
                    {
                        Program.straightCount++;
                    }
                }
                else if (statMode == StatMode.AllPlayers)
                {
                    foreach (var player in testTable.players)
                    {
                        if (player.handType == HandType.FullHouse)
                        {
                            Program.fullHouseCount++;
                        }else if (player.handType == HandType.StraightFlush)
                        {
                            Program.straightFlushCount++;
                        }else if (player.handType == HandType.FourOfAKind)
                        {
                            Program.fourOfAKindCount++;
                        }else if (player.handType == HandType.Flush)
                        {
                            Program.flushCount++;
                        }else if (player.handType == HandType.Straight)
                        {
                            Program.straightCount++;
                        }
                    }
                }

                testTable.ClearCards();
                count++;
            } while (count != 10000);

            if (statMode != StatMode.Disabled)
            {
                Console.WriteLine("Straight flush count: " + straightFlushCount);
                Console.WriteLine("Four of a kind count: " + fourOfAKindCount);
                Console.WriteLine("Full house count: " + fullHouseCount);
                Console.WriteLine("Flush count: " + flushCount);
                Console.WriteLine("Straight count: " + straightCount);
            }
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
                    if (Program.statMode == StatMode.Disabled)
                    {
                        Console.WriteLine("Winner: " + player.name);
                    }else if (Program.statMode == StatMode.AllWinners)
                    {
                        if (player.handType == HandType.FullHouse)
                        {
                            Program.fullHouseCount++;
                        }else if (player.handType == HandType.StraightFlush)
                        {
                            Program.straightFlushCount++;
                        }else if (player.handType == HandType.FourOfAKind)
                        {
                            Program.fourOfAKindCount++;
                        }else if (player.handType == HandType.Flush)
                        {
                            Program.flushCount++;
                        }else if (player.handType == HandType.Straight)
                        {
                            Program.straightCount++;
                        }
                    }
                }
            }
        }
    }

    public class PokerGame
    {
        public static Tuple<HandType, int> GetBestHand(List<Card> hand, List<Card> board)
        {
            List<Card> cards = new List<Card>();
            List<Card> cardsDesc = new List<Card>();
            board.ForEach(x => cards.Add(x));
            hand.ForEach(x => cards.Add(x));
            cards.FindAll(x => x.value == 1).ToList().ForEach(x => cards.Add(new Card(){color = x.color, ID = x.ID, value = 14}));

            cards = cards.OrderBy(x => x.value).ToList();
            cardsDesc = cards.OrderByDescending(x => x.value).ToList();
            
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
            for (int i = 0; i < cardsDesc.Count; i++)
            {
                Card card = cardsDesc[i];

                if (i == cardsDesc.Count - 1)
                {
                    if (chain == 3)
                    {
                        return new Tuple<HandType, int>(HandType.FourOfAKind, card.value * 4000 + cards[cards.Count-5].value);
                    }
                    break;
                }

                if (card.value == cardsDesc[i + 1].value)
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
            List<Card> threeOfAKind = new List<Card>();
            chain = 0;
            //Find three of a kind
            for (int i = 0; i < cardsDesc.Count; i++)
            {
                Card card = cardsDesc[i];

                if (i == cardsDesc.Count - 1)
                {
                    if (chain == 2)
                    {
                        threeOfAKind.Add(card);
                        threeOfAKind.Add(cardsDesc[i - 1]);
                        threeOfAKind.Add(cardsDesc[i - 2]);
                    }
                    break;
                }

                if (card.value == cardsDesc[i + 1].value)
                {
                    chain++;
                }
                else
                {
                    if (chain == 2)
                    {
                        threeOfAKind.Add(card);
                        threeOfAKind.Add(cardsDesc[i - 1]);
                        threeOfAKind.Add(cardsDesc[i - 2]);
                        break;
                    }
                    
                    chain = 0;
                }
            }

            //Find a pair
            chain = 0;
            if (threeOfAKind.Count == 3)
            {
                for (int i = 0; i < cardsDesc.Count; i++)
                {
                    Card card = cardsDesc[i];

                    if (i == cardsDesc.Count - 1)
                    {
                        if (chain == 1)
                        {
                            return new Tuple<HandType, int>(HandType.FullHouse, threeOfAKind[0].value * 30000 + threeOfAKind[1].value * 30000 + threeOfAKind[2].value * 30000 + card.value * 200 + cardsDesc[i-1].value * 200); 
                        }
                        break;
                    }

                    if (card.value == cardsDesc[i + 1].value && threeOfAKind.All(x => x != card))
                    {
                        chain++;
                    }
                    else
                    {
                        if (chain == 1)
                        {
                            return new Tuple<HandType, int>(HandType.FullHouse, threeOfAKind[0].value * 30000 + threeOfAKind[1].value * 30000 + threeOfAKind[2].value * 30000 + card.value * 200 + cardsDesc[i-1].value * 200);
                        }
                    
                        chain = 0;
                    }
                }
            }

            //Flush
            List<Card> cardsByColor = cards.OrderBy(x => x.color).ToList();
            chain = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cardsByColor[i];

                if (i == cards.Count - 1)
                {
                    if (chain == 4)
                    {
                        int handValue = 0;
                        List<Card> flushHand = cards.FindAll(x => x.color == card.color).ToList().OrderByDescending(x => x.value).ToList();
                        for (int j = 1; j < 6; j++)
                        {
                            if (flushHand[j-1].value == 1)
                            {
                                handValue += flushHand[j-1].value * j * 10000;
                            }
                            else
                            {
                                handValue += flushHand[j-1].value * j * 1000;
                            }
                        }
                        return new Tuple<HandType, int>(HandType.Flush, handValue);
                    }
                    break;
                }
                
                if (card.color == cardsByColor[i + 1].color && card.value != 1)
                {
                    chain++;
                }
                else
                {
                    if (chain == 4)
                    {
                        int handValue = 0;
                        List<Card> flushHand = cards.FindAll(x => x.color == card.color).ToList().OrderByDescending(x => x.value).ToList();
                        for (int j = 1; j < 6; j++)
                        {
                            if (flushHand[j-1].value == 1)
                            {
                                handValue += flushHand[j-1].value * j * 10000;
                            }
                            else
                            {
                                handValue += flushHand[j-1].value * j * 1000;
                            }
                        }
                        return new Tuple<HandType, int>(HandType.Flush, handValue);
                    }
                    chain = 0;
                }
            }
            
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
            for (int i = 0; i < cardsDesc.Count; i++)
            {
                Card card = cardsDesc[i];

                if (i == cards.Count - 1)
                {
                    if (chain == 2)
                    {
                        return new Tuple<HandType, int>(HandType.ThreeOfAKind, card.value * 3000 + cards[cards.Count-3].value + cards[cards.Count-4].value);
                    }
                    break;
                }
                
                //Console.WriteLine(card.value + " " + card.color + " " + cards[i + 1].value +  " " + cards[i+1].color);
                
                if (card.value == cardsDesc[i + 1].value)
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
            List<Card> firstPair = new List<Card>();
            chain = 0;
            for (int i = 0; i < cardsDesc.Count; i++)
            {
                Card card = cardsDesc[i];

                if (i == cardsDesc.Count - 1)
                {
                    if (chain == 1)
                    {
                        firstPair.Add(card);
                        firstPair.Add(cardsDesc[i-1]);
                    }
                    break;
                }

                if (card.value == cardsDesc[i + 1].value)
                {
                    chain++;
                }
                else
                {
                    if (chain == 1)
                    {
                        firstPair.Add(card);
                        firstPair.Add(cardsDesc[i - 1]);
                        break;
                    }
                    
                    chain = 0;
                }
            }

            chain = 0;
            for (int i = 0; i < cardsDesc.Count; i++)
            {
                Card card = cardsDesc[i];

                if (i == cardsDesc.Count - 1)
                {
                    if (chain == 1)
                    {
                        return new Tuple<HandType, int>(HandType.TwoPairs, firstPair[0].value * 20000 + firstPair[1].value * 20000 + card.value * 200 + cardsDesc[i-1].value * 200 + cardsDesc.FirstOrDefault(x => x != firstPair[0] && x != firstPair[1] && x != card && x != cardsDesc[i-1]).value); 
                    }
                    break;
                }

                if (card.value == cardsDesc[i + 1].value && firstPair.All(x => x != card) && card.value != 1 && cardsDesc[i + 1].value != 1)
                {
                    chain++;
                }
                else
                {
                    if (chain == 1)
                    {
                        return new Tuple<HandType, int>(HandType.TwoPairs, firstPair[0].value * 20000 + firstPair[1].value * 20000 + card.value * 200 + cardsDesc[i-1].value * 200 + cardsDesc.FirstOrDefault(x => x != firstPair[0] && x != firstPair[1] && x != card && x != cardsDesc[i-1]).value);
                    }
                    
                    chain = 0;
                }
            }
            
            // Pair
            chain = 0;
            for (int i = 0; i < cardsDesc.Count; i++)
            {
                Card card = cardsDesc[i];

                if (i == cardsDesc.Count - 1)
                {
                    if (chain == 1)
                    {
                        return new Tuple<HandType, int>(HandType.Pair, card.value * 2000 + cards[cards.Count-2].value * 100 + cards[cards.Count-3].value * 10 + cards[cards.Count-4].value);
                    }
                    break;
                }

                if (card.value == cardsDesc[i + 1].value)
                {
                    chain++;
                }
                else
                {
                    if (chain == 1)
                    {
                        if (card.value == 1)
                        {
                            return new Tuple<HandType, int>(HandType.Pair, card.value * 20000 + cards[cards.Count-1].value * 100 + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1]).value * 10 + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1] && x != cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1])).value);
                        }
                        return new Tuple<HandType, int>(HandType.Pair, card.value * 2000 + cards[cards.Count-1].value * 100 + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1]).value * 10 + cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1] && x != cards.OrderByDescending(x => x.value).ToList().FirstOrDefault(x => x.value != card.value && x != cards[cards.Count-1])).value);
                    }
                    chain = 0;
                }
            }
            
            // High
            return new Tuple<HandType, int>(HandType.High, cards[cards.Count - 1].value * 10000 + cards[cards.Count - 2].value * 1000 + cards[cards.Count - 3].value * 100 + cards[cards.Count - 4].value * 10 + cards[cards.Count - 5].value);
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

    public enum StatMode
    {
        PlayMode,
        Disabled,
        Player1,
        AllWinners,
        AllPlayers
    }
}