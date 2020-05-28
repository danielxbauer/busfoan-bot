using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Busfoan.Domain;
using Busfoan.Graphic.Extensions;
using Busfoan.Graphic.Models;
using static Busfoan.Graphic.Util.ImageUtil;

namespace Busfoan.Graphic.Services
{
    public class ImageProcessor : IImageProcessor
    {
        private const string back = "back";

        private readonly int cardWidth;
        private readonly int cardHeight;
        private readonly IDictionary<string, Bitmap> imageCache;

        public ImageProcessor(string assetPath, IEnumerable<Card> cards)
        {
            var backImage = new Bitmap($"{assetPath}/{back}.png");
            cardWidth = backImage.Width;
            cardHeight = backImage.Height;

            imageCache = LoadCardImages(assetPath, cards);
            imageCache.Add("back", backImage);
        }

        internal ImageProcessor(Bitmap bitmap, Card card)
        {
            cardWidth = bitmap.Width;
            cardHeight = bitmap.Height;

            imageCache = new Dictionary<string, Bitmap>
            {
                {  card.Id, bitmap }
            };
            imageCache.Add("back", bitmap);
        }

        private Dictionary<string, Bitmap> LoadCardImages(string assetPath, IEnumerable<Card> cards)
        {
            var images = new Dictionary<string, Bitmap>();
            foreach (var card in cards)
            {
                var image = new Bitmap($"{assetPath}/{card.Id}.png");
                if (image.Width != cardWidth || image.Height != cardHeight)
                    throw new ArgumentException($"Image '{assetPath}/{card.Id}.png' has another width/height.");

                images.Add(card.Id, image);
            }

            return images;
        }

        private Bitmap ToImage(Card card)
        {
            return card.IsRevealed
                ? imageCache[card.Id]
                : imageCache["back"];
        }

        public Stream GenerateCardImage(IEnumerable<Card> cards)
        {
            var options = new MergeOptions
            {
                MinWidth = cardWidth * 4,
                Gap = 20
            };

            var images = cards.Select(ToImage).ToList();
            return Horizontal(options, images.ToArray()).AsStream();
        }

        public Stream GeneratePyramidImage(Pyramid pyramid)
        {
            var gap20 = new MergeOptions { Gap = 20 };

            var rowImages = new List<Bitmap>();
            foreach (var row in pyramid.Rows)
            {
                var images = row.Cards.Select(ToImage).ToList();
                var rowImage = Horizontal(gap20, images.ToArray());
                rowImages.Add(rowImage);
            }

            var centered = new MergeOptions { Gap = 20, XAlign = XAlign.Center };
            return Vertical(centered, rowImages.ToArray()).AsStream();
        }
    }
}
