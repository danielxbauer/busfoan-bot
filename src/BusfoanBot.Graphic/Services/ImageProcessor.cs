using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BusfoanBot.Domain;
using BusfoanBot.Graphic.Extensions;
using BusfoanBot.Graphic.Models;

namespace BusfoanBot.Graphic.Services
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

        public Stream GenerateCardImage(IEnumerable<Card> cards, bool showEmptyCard)
        {
            var images = cards.Select(card => imageCache[card.Id].WithNoPadding()).ToList();

            var options = new MergeOptions
            {
                Padding = Padding.All(20),
                Gap = 20
            };

            if (showEmptyCard)
                images.Add(imageCache["back"].WithNoPadding());

            options.Padding.Right += options.Gap + (cardWidth * (4 - images.Count()));

            return ImageUtil.MergeHorizontal(images, options).AsStream();
        }
    }
}
