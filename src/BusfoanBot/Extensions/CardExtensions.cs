using System;
using System.Collections.Generic;
using System.Text;
using BusfoanBot.Models;

namespace BusfoanBot.Extensions
{
    internal static class CardExtensions
    {
        public static string ToFilePath(this Card card)
        {
            string symbolName = string.Empty;
            switch (card.Symbol)
            {
                case CardSymbol.Club: symbolName = "clubs"; break;
                case CardSymbol.Diamond: symbolName = "diamonds"; break;
                case CardSymbol.Heart: symbolName = "hearts"; break;
                case CardSymbol.Spade: symbolName = "spades"; break;
            }

            return @$"Assets\{symbolName}\{card.Type}_of_{symbolName}.png";
        }
    }
}
