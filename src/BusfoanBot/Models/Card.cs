using System;

namespace BusfoanBot.Models
{
    public sealed class Card
    {
        public Card(string type, CardSymbol symbol, int value)
        {
            Type = type;
            Symbol = symbol;
            Value = value;
        }

        public string Id => $"{MapSymbol(Symbol)}{Type}";

        // 2 3 4 5 6 7 8 9 10 J Q K A
        public string Type { get; }
        public CardSymbol Symbol { get; }
        public int Value { get; }

        public bool IsRed => Symbol == CardSymbol.Diamond || Symbol == CardSymbol.Heart;
        public bool IsBlack => !IsRed;

        public override string ToString() => $"{Type}{MapSymbol(Symbol)}";

        private string MapSymbol(CardSymbol symbol) => symbol switch
        {
            CardSymbol.Club => "C",
            CardSymbol.Diamond => "D",
            CardSymbol.Heart => "H",
            CardSymbol.Spade => "S",
            _ => throw new ArgumentException($"No CardSymbol '{symbol}' to mapped.")
        };
    }
}
