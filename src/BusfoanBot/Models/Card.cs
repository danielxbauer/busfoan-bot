﻿using System;
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

        public string ToFilePath()
        {
            string symbolName = string.Empty;
            switch (Symbol)
            {
                case CardSymbol.Club: symbolName = "clubs"; break;
                case CardSymbol.Diamond: symbolName = "diamonds"; break;
                case CardSymbol.Heart: symbolName = "hearts"; break;
                case CardSymbol.Spade: symbolName = "spades"; break;
            }

            //return @"C:\Dev\busfoan-bot\src\BusfoanBot\Assets\10_of_clubs.png";
            return @$"Assets\{symbolName}\{Type}_of_{symbolName}.png";
        }
    }
}
