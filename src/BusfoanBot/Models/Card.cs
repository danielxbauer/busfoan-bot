using System;
using Discord;

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
        public Card(string type, CardSymbol symbol, int value)
        {
            Type = type;
            Symbol = symbol;
            Value = value;
        }

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
