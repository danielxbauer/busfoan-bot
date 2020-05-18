using System;
using Discord;

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

        public string Id => $"{Type}{MapSymbolToEmote(Symbol)}";

        // 2 3 4 5 6 7 8 9 10 J Q K A
        public string Type { get; }
        public CardSymbol Symbol { get; }
        public int Value { get; }

        public bool IsRed => Symbol == CardSymbol.Diamond || Symbol == CardSymbol.Heart;
        public bool IsBlack => !IsRed;

        public override string ToString() => $"{Type}{MapSymbolToEmote(Symbol)}";

        // TODO: to extension method
        private IEmote MapSymbolToEmote(CardSymbol symbol)
        {
            switch(symbol)
            {
                case CardSymbol.Club: return Emotes.Club;
                case CardSymbol.Diamond: return Emotes.Diamond;
                case CardSymbol.Heart: return Emotes.Heart;
                case CardSymbol.Spade: return Emotes.Spade;
                default: throw new ArgumentException($"No emoji mapped to CardSymbol '{symbol}'");
            }
        }
    }
}
