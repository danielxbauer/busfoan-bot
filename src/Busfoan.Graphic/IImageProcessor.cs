using System.Collections.Generic;
using System.IO;
using Busfoan.Domain;

namespace Busfoan.Graphic
{
    public interface IImageProcessor
    {
        Stream GenerateCardImage(IEnumerable<Card> cards, bool showEmptyCard);
    }
}
