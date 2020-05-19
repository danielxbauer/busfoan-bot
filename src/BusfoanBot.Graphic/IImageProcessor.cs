using System.Collections.Generic;
using System.IO;
using BusfoanBot.Domain;

namespace BusfoanBot.Graphic
{
    public interface IImageProcessor
    {
        Stream GenerateCardImage(IEnumerable<Card> cards, bool showEmptyCard);
    }
}
