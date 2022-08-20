using Cornice.Models;

namespace Cornice;

public class GameManager
{
    private Board Board { get; set; }
    private Game Game { get; set; }
    
    public GameManager()
    {
        Board = new Board();
        Game = new Game();
    }
    
    public void PlayGame()
    {
        Board.Draw();
        do
        {
            StageOne();
            StageTwo();
        } while (!Game.Ended());
        
        Console.Clear();
        Console.WriteLine(Game.Lose() ? "You lose" : "You win!");
    }

    private void StageOne()
    {
        do
        {
            var currentCard = Game.GetCurrentCard();
            if(currentCard is null) return;
            Board.UpdateDeck(currentCard);
            if (!Game.CanPlaceCard())
            {
                // End game in case of a figure can't be placed
                Game.Lose(true);
                return;
            }
            
            (int, int) selectedSlot;
            do
            {
                selectedSlot = Board.MoveCourser(3, 2, new[] { 0, 9, 0, 5 });
            } while (Board.BrokeBoardRules(selectedSlot, currentCard));

            var canPlaceCard = Game.PlaceCard(selectedSlot);
            if (canPlaceCard)
                Board.PlaceCard(selectedSlot, currentCard);
        } while (!Game.CheckIfBoardIsFull());
        
        var lastCard = Game.GetCurrentCard();
        if(lastCard is null) return;
        Board.UpdateDeck(lastCard);
    }
    private void StageTwo()
    {
        if(Game.Lose())
            return;
        
        do
        {
            var selectedCards = new List<(Card card, (int, int) position)>();

            do
            {
                (int, int) selectedCardPosition;
                Card selectedCard;
                do
                {
                    selectedCardPosition = Board.MoveCourser(3, 2, new[] { 0, 9, 0, 5 });
                    selectedCard = Game.GetCardByPosition(selectedCardPosition);
                } while (Board.BrokeCleanBoardRules(selectedCard));
                selectedCards.Add((selectedCard, selectedCardPosition));
                if(Board.BrokeSelectionBoardRules(selectedCards.Select(sc => sc.card)))
                    selectedCards.Clear();
            } while (Board.BrokeSumRules(selectedCards.Select(sc => sc.card)));

            Game.RemoveCards(selectedCards.Select(sc => sc.position));
            Board.RemoveCards(selectedCards.Select(sc => sc.position));

        } while (!Game.CheckIfBoardIsCleaned());

        if (Game.CheckIfBoardIsFull())
            // End game you can't discard any cards
            Game.Lose(true);
    }
}