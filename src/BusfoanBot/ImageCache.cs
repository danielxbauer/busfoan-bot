using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BusfoanBot.Extensions;
using BusfoanBot.Graphic;
using BusfoanBot.Graphic.Extensions;
using BusfoanBot.Graphic.Models;
using BusfoanBot.Models;

namespace BusfoanBot
{
    public class ImageCache
    {
        private IDictionary<string, Bitmap> imageCache;

        public ImageCache()
        {
            imageCache = new Dictionary<string, Bitmap>();
            foreach(var card in BotActions.GenerateCards()) 
            {
                imageCache.Add(card.Id, new Bitmap(card.ToFilePath()));
            }

            imageCache.Add("back", new Bitmap("Assets/back.png"));
        }

        public Stream GenerateCardImage(IEnumerable<Card> cards, bool showEmptyCard)
        {
            var images = cards
                .Select(card => imageCache[card.Id])
                .Select(i => i.WithNoPadding())
                .ToList();

            var options = new MergeOptions
            {
                Padding = Padding.All(20),
                Gap = 20
            };

            if (showEmptyCard)
                images.Add(imageCache["back"].WithNoPadding());

            options.Padding.Right += options.Gap + (500 * (4 - images.Count()));

            return ImageUtil.MergeHorizontal(images, options).AsStream();
        }

    }
}
