﻿using System;
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
            var images = cards.Select(card => imageCache[card.Id]).ToList();
            if (showEmptyCard) images.Add(imageCache["back"]);

            var options = new MergeOptions 
            { 
                MinWidth = cardWidth * 4,
                Gap = 20 
            };

            return Horizontal(options, images.ToArray()).AsStream();
        }
    }
}
