namespace BusfoanBot.Models
{
    public enum CardSymbol
    {
        Club = 1,
        Spade = 2,
        Diamond = 3,
        Heart = 4
    }

    public sealed class Card
    {
        public Card(string type, CardSymbol symbol)
        {
            Type = type;
            Symbol = symbol;
        }

        public string Type { get; }
        public CardSymbol Symbol { get; }
        public bool IsRed => Symbol == CardSymbol.Diamond || Symbol == CardSymbol.Heart;
        public bool IsBlack => !IsRed;

        public override string ToString() => $"{Type}{Symbol}";
    }
}
