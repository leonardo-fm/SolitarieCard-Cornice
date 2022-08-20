using Cornice.Models;

namespace Cornice;

public class Board
{
    private string _playingBoard = "┌──┬──┬──┬──┐\n" + 
                                   "│  │  │  │  │\n" + 
                                   "├──┼──┼──┼──┤\n" + 
                                   "│  │  │  │  │\n" +
                                   "├──┼──┼──┼──┤\n" + 
                                   "│  │  │  │  │\n" + 
                                   "├──┼──┼──┼──┤\n" + 
                                   "│  │  │  │  │\n" +
                                   "└──┴──┴──┴──┘\n";

    public static readonly (int x, int y)[,] PlayingBoardPositions =
    {
        { (1, 1), (4, 1), (7, 1), (10, 1) },
        { (1, 3), (4, 3), (7, 3), (10, 3) },
        { (1, 5), (4, 5), (7, 5), (10, 5) },
        { (1, 7), (4, 7), (7, 7), (10, 7) }
    };

    private string _cardStash = "   ┌──┬──┐   \n" + 
                                "   │  │  │   \n" + 
                                "   └──┴──┘   ";

    private (int x, int y)[] _cardStashPositions =
    {
        (4, 10), (7, 10) // Deck, other
    };

    private (int x, int y) _playerPosition = (1, 1);

    public void Draw()
    {
        Console.WriteLine(_playingBoard + _cardStash);
    }

    public (int x, int y) MoveCourser(int sideStep = 1, int verticalStep = 1, int[]? boundary = null)
    {
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (Console.CursorLeft - sideStep >= (boundary != null ? boundary[0] : 0))
                        {
                            Console.SetCursorPosition(Console.CursorLeft - sideStep, Console.CursorTop);
                            _playerPosition = Console.GetCursorPosition();
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (Console.CursorLeft <= (boundary != null ? boundary[1] : 0))
                        {
                            Console.SetCursorPosition(Console.CursorLeft + sideStep, Console.CursorTop);
                            _playerPosition = Console.GetCursorPosition();
                        }

                        break;
                    case ConsoleKey.UpArrow:
                        if (Console.CursorTop - verticalStep >= (boundary != null ? boundary[2] : 0))
                        {
                            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - verticalStep);
                            _playerPosition = Console.GetCursorPosition();
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (Console.CursorTop <= (boundary != null ? boundary[3] : 0))
                        {
                            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + verticalStep);
                            _playerPosition = Console.GetCursorPosition();
                        }

                        break;
                    case ConsoleKey.Enter:
                        return Console.GetCursorPosition();
                }
            }
        }
    }

    public void PlaceCard((int x, int y) position, Card card)
    {
        Console.SetCursorPosition(position.x, position.y);
        Console.Write(card.Name);
    }
    public void RemoveCards(IEnumerable<(int x, int y)> positions)
    {
        foreach (var position in positions)
        {
            Console.SetCursorPosition(position.x, position.y);
            Console.Write("  ");
            Console.SetCursorPosition(_playerPosition.x, _playerPosition.y);
        }
    }

    public void UpdateDeck(Card card)
    {
        Console.SetCursorPosition(_cardStashPositions[0].x, _cardStashPositions[0].y);
        Console.Write(card.Name);
        Console.SetCursorPosition(_playerPosition.x, _playerPosition.y);
    }

    public bool BrokeBoardRules((int x, int y) position, Card card)
    {
        if (card.Values == CardValues.Jack)
        {
            var availableSpacesJack = new []{ (1, 3), (10, 3), (1, 5), (10, 5) };
            return Array.IndexOf(availableSpacesJack, position) == -1;
        }
        if (card.Values == CardValues.Queen)
        {
            var availableSpacesQueen = new []{ (4, 1), (7, 1), (4, 7), (7, 7) };
            return Array.IndexOf(availableSpacesQueen, position) == -1;
        }
        if (card.Values == CardValues.King)
        {
            var availableSpacesKing = new []{ (1, 1), (10, 1), (1, 7), (10, 7) };
            return Array.IndexOf(availableSpacesKing, position) == -1;
        }

        return false;
    }
    public bool BrokeCleanBoardRules(Card? card)
    {
        return card is null || card.Values is CardValues.Jack or CardValues.Queen or CardValues.King;
    }
    public bool BrokeSelectionBoardRules(IEnumerable<Card> cards)
    {
        var cardsArray = cards as Card[] ?? cards.ToArray();
        var response = cardsArray.Length switch
        {
            0 => false,
            1 => false,
            2 => cardsArray[0].Equals(cardsArray[1]) || (int)cardsArray[0].Values + (int)cardsArray[1].Values != 10,
            _ => true,
        };
        
        return response;
    }
    public bool BrokeSumRules(IEnumerable<Card> cards)
    {
        var totalSum = 0;
        foreach (var card in cards)
        {
            totalSum += (int)card.Values;
        }

        return totalSum != 10;
    }

}