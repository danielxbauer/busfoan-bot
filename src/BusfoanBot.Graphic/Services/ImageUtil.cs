using System;
using System.Drawing;
using System.Linq;
using BusfoanBot.Graphic.Extensions;
using BusfoanBot.Graphic.Models;

namespace BusfoanBot.Graphic.Services
{
    public enum XAlign
    {
        Left, Center, Right
    }

    public enum YAlign
    {
        Top, Center, Bottom
    }

    public class MergeOptions
    {
        public int MinWidth { get; set; } = 0;
        public int MinHeight { get; set; } = 0;
        public Padding Padding { get; set; } = Padding.None;
        public int Gap { get; set; } = 0;
        public XAlign XAlign { get; set; } = XAlign.Center;
        public YAlign YAlign { get; set; } = YAlign.Center;
    }

    public static class ImageUtil
    {
        public static Bitmap Vertical(params Bitmap[] images)
            => Vertical(null, images);
        public static Bitmap Vertical(MergeOptions options, params Bitmap[] images)
        {
            if (images == null || images.Count() == 0) return null;
            options = options ?? new MergeOptions();

            int width = images.Select(i => i.Width).Max()
                      + options.Padding.Width;

            width = Math.Max(width, options.MinWidth);

            int height = images.Sum(i => i.Height)
                      + options.Padding.Height
                      + (options.Gap * (images.Count() - 1)); // only between images

            height = Math.Max(height, options.MinHeight);

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                //int heightOffset = options.Padding.Top;
                int heightOffset = options.YAlign switch
                {
                    YAlign.Top => options.Padding.Top,
                    YAlign.Center => (height - images.Height()) / 2,
                    YAlign.Bottom => height - images.Height() - options.Padding.Bottom,
                    _ => throw new NotImplementedException()
                };

                foreach (var image in images)
                {
                    int widthOffset = options.XAlign switch
                    {
                        XAlign.Left => options.Padding.Left,
                        XAlign.Center => (width - image.Width) / 2,
                        XAlign.Right => width - image.Width - options.Padding.Right,
                        _ => throw new NotImplementedException()
                    };

                    g.DrawImage(image,
                        widthOffset, heightOffset,
                        image.Width, image.Height);

                    heightOffset += image.Height + options.Gap;
                }
            }

            return bitmap;
        }

        public static Bitmap Horizontal(params Bitmap[] images)
            => Horizontal(null, images);

        public static Bitmap Horizontal(MergeOptions options, params Bitmap[] images)
        {
            if (images == null || images.Count() == 0) return null;
            options = options ?? new MergeOptions();

            int width = images.Width()                        
                      + options.Padding.Width
                      + (options.Gap * (images.Count() - 1)); // between images

            width = Math.Max(width, options.MinWidth);

            int height = images.Select(i => i.Height).Max() 
                      + options.Padding.Height;

            height = Math.Max(height, options.MinHeight);
                        
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int widthOffset = options.XAlign switch
                {
                    XAlign.Left => options.Padding.Left,
                    XAlign.Center => (width - images.Width()) / 2,
                    XAlign.Right => width - images.Width() - options.Padding.Right,
                    _ => throw new NotImplementedException()
                };

                foreach (var image in images)
                {
                    int heightOffset = options.YAlign switch
                    {
                        YAlign.Top => options.Padding.Top,
                        YAlign.Center => (height - image.Height) / 2,
                        YAlign.Bottom => height - image.Height - options.Padding.Bottom,
                        _ => throw new NotImplementedException()
                    };

                    g.DrawImage(image, 
                        widthOffset, heightOffset, 
                        image.Width, image.Height);

                    widthOffset += image.Width + options.Gap;
                }
            }

            return bitmap;
        }
    }
}
