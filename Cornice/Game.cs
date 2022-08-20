using Cornice.Models;

namespace Cornice;

public class Game
{
    private static List<Card> _deck = new();
    private static Card?[,]? _gameBoard;
    private static List<Card> _wasteDeck = new();

    private bool _isBoardFull;
    private bool _isGameLost;
    private Random _random = new();

    public Game()
    {
        _deck = GenerateDeck();
        _wasteDeck = new List<Card>();
        _gameBoard = new Card[4, 4];
        _isBoardFull = false;
        _isGameLost = false;
    }

    public Card? GetCurrentCard()
    {
        return _deck.Count == 0 ? null : _deck.First();
    }

    public Card GetCardByPosition((int x, int y) position)
    {
        var index = GetCardIndex(position);
        if (position == (-1, -1)) throw new NullReferenceException();
        return _gameBoard[index.x, index.y]!;
    }

    public bool CanPlaceCard()
    {
        var currentCard = GetCurrentCard();
        switch (currentCard.Values)
        {
            case CardValues.Jack:
            {
                var jacksValidPositions = new[] { (1, 0), (2, 0), (1, 3), (2, 3) };
                foreach (var validPosition in jacksValidPositions)
                {
                    if (_gameBoard[validPosition.Item1, validPosition.Item2] is null)
                        return true;
                }

                break;
            }
            case CardValues.Queen:
            {
                var queensValidPositions = new[] { (0, 1), (0, 2), (3, 1), (3, 2) };
                foreach (var validPosition in queensValidPositions)
                {
                    if (_gameBoard[validPosition.Item1, validPosition.Item2] is null)
                        return true;
                }

                break;
            }
            case CardValues.King:
            {
                var kingsValidPositions = new[] { (0, 0), (3, 0), (0, 3), (3, 3) };
                foreach (var validPosition in kingsValidPositions)
                {
                    if (_gameBoard[validPosition.Item1, validPosition.Item2] is null)
                        return true;
                }

                break;
            }
            default:
                return true;
        }

        return false;
    }

    public bool PlaceCard((int x, int y) position)
    {
        var index = GetCardIndex(position);
        if (index == (-1, -1)) throw new ArgumentException();
        if (_gameBoard[index.x, index.y] != null)
            return false;

        var card = GetCurrentCard();
        _gameBoard[index.x, index.y] = card;
        _deck.Remove(card);
        return true;
    }

    public bool RemoveCards(IEnumerable<(int x, int y)> positions)
    {
        foreach (var position in positions)
        {
            var index = GetCardIndex(position);
            if (index == (-1, -1)) throw new ArgumentException();
            if (_gameBoard[index.x, index.y] == null)
                return false;

            _wasteDeck.Add(_gameBoard[index.x, index.y]!);
            _gameBoard[index.x, index.y] = null;
        }

        return true;
    }

    public bool CheckIfBoardIsFull()
    {
        for (int x = 0; x < _gameBoard.GetLength(0); x++)
        {
            for (int y = 0; y < _gameBoard.GetLength(1); y++)
            {
                if (_gameBoard[x, y] == null)
                {
                    _isBoardFull = false;
                    return _isBoardFull;
                }
            }
        }

        _isBoardFull = true;
        return _isBoardFull;
    }

    public bool CheckIfBoardIsCleaned()
    {
        var cardNumbers = new List<int>();
        for (int x = 0; x < _gameBoard.GetLength(0); x++)
        {
            for (int y = 0; y < _gameBoard.GetLength(1); y++)
            {
                if (_gameBoard[x, y] is not null && (int)_gameBoard[x, y]!.Values <= 10)
                    cardNumbers.Add((int)_gameBoard[x, y]!.Values);
            }
        }

        for (var i = 0; i < cardNumbers.Count; i++)
        {
            for (var j = 0; j < cardNumbers.Count; j++)
            {
                if (i != j && cardNumbers[i] + cardNumbers[j] == 10)
                    return false;
            }
        }

        return true;
    }

    public bool Lose(bool lost = false)
    {
        if (lost)
            _isGameLost = true;

        return _isGameLost;
    }
    public bool Ended()
    {
        var isWin = CheckForWin();
        var isLose = Lose();
        return isWin != isLose;
    }
    
    public bool CheckForWin()
    {
        var jacksValidPositions = new[] { (0, 1), (0, 2), (3, 1), (3, 2) };
        var queensValidPositions = new[] { (1, 0), (2, 0), (1, 3), (2, 3) };
        var kingsValidPositions = new[] { (0, 0), (0, 3), (3, 0), (3, 3) };

        foreach (var jacksValidPosition in jacksValidPositions)
        {
            if (_gameBoard[jacksValidPosition.Item1, jacksValidPosition.Item2] is null ||
                _gameBoard[jacksValidPosition.Item1, jacksValidPosition.Item2]!.Values != CardValues.Jack)
                return false;
        }
        foreach (var queensValidPosition in queensValidPositions)
        {
            if (_gameBoard[queensValidPosition.Item1, queensValidPosition.Item2] is null ||
                _gameBoard[queensValidPosition.Item1, queensValidPosition.Item2]!.Values != CardValues.Queen)
                return false;
        }
        foreach (var kingsValidPosition in kingsValidPositions)
        {
            if (_gameBoard[kingsValidPosition.Item1, kingsValidPosition.Item2] is null ||
                _gameBoard[kingsValidPosition.Item1, kingsValidPosition.Item2]!.Values != CardValues.King)
                return false;
        }

        return _wasteDeck.Count == 40; // Total number of cards without figures
    }

    private static (int x, int y) GetCardIndex((int x, int y) position)
    {
        for (int x = 0; x < Board.PlayingBoardPositions.GetLength(0); x++)
        {
            for (int y = 0; y < Board.PlayingBoardPositions.GetLength(1); y++)
            {
                if (Board.PlayingBoardPositions[x, y] == position)
                    return (x, y);
            }
        }

        return (-1, -1);
    }

    private List<Card> GenerateDeck()
    {
        var deck = new List<Card>();
        foreach (var cardValue in Enum.GetValues(typeof(CardValues)))
        {
            foreach (var cardSuit in Enum.GetValues(typeof(CardSuits)))
            {
                deck.Add(new Card((CardSuits)cardSuit, (CardValues)cardValue));
            }
        }

        Shuffle(deck);
        return deck;
    }

    private void Shuffle<T>(IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = _random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}