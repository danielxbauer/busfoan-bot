using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BusfoanBot.Graphic.Models;

namespace BusfoanBot.Graphic.Services
{
    internal class MergeOptions
    {
        public Padding Padding { get; set; } = Padding.None;
        public int Gap { get; set; } = 0;
    }

    internal static class ImageUtil
    {
        public static Bitmap MergeHorizontal(IEnumerable<PaddableImage> images, MergeOptions options)
        {
            if (images == null || images.Count() == 0) return null;

            int width = images.Sum(i => i.Width)                        
                      + options.Padding.Width
                      + (options.Gap * (images.Count() - 1)); // between images

            int height = images.Select(i => i.Height).Max() 
                      + options.Padding.Height;
            
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int widthOffset = options.Padding.Left;
                int heightOffset = options.Padding.Top;

                foreach (var image in images)
                {
                    g.DrawImage(image.Image, 
                        widthOffset + image.Padding.Left, 
                        heightOffset + image.Padding.Top, 
                        image.Image.Width, 
                        image.Image.Height);

                    widthOffset += image.Width + options.Gap;
                }
            }

            return bitmap;
        }
    }
}
