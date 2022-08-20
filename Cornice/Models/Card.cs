namespace Cornice.Models;

public class Card
{
    public static char[] ValuesAbb = new[] { 'A', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'J', 'Q', 'K' };
    public static char[] SuitAbb = new[] { 'C', 'D', 'H', 'S' }; 
    
    public Card(CardSuits suits, CardValues values)
    {
        Suits = suits;
        Values = values;
        Name = $"{SuitAbb[(int)suits]}{ValuesAbb[(int)values - 1]}";
    }
    
    public CardSuits Suits { get; set; }
    public CardValues Values { get; set; }
    public string Name { get; set; }
}