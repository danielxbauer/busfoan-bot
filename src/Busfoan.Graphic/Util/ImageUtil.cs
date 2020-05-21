using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using Busfoan.Graphic.Extensions;
using Busfoan.Graphic.Models;

namespace Busfoan.Graphic.Util
{
    internal static class ImageUtil
    {
        public static Bitmap Pad(Bitmap image, Padding padding)
        {
            if (image == null) return null;
            if (padding == null) return image;

            int width = image.Width + padding.Width;
            int height = image.Height + padding.Height;

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image,
                    padding.Left, padding.Top,
                    image.Width, image.Height);
            }

            return bitmap;
        }

        public static Bitmap Vertical(params Bitmap[] images)
            => Vertical(null, images);
        public static Bitmap Vertical(MergeOptions options, params Bitmap[] images)
        {
            if (images == null || images.Count() == 0) return null;
            options = options ?? new MergeOptions();

            int width = images.Select(i => i.Width).Max();
            width = Math.Max(width, options.MinWidth);

            int height = images.Sum(i => i.Height)
                + (options.Gap * (images.Count() - 1)); // only between images
            height = Math.Max(height, options.MinHeight);

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int heightOffset = options.YAlign switch
                {
                    YAlign.Top => 0,
                    YAlign.Center => (height - images.Height()) / 2,
                    YAlign.Bottom => height - images.Height(),
                    _ => throw new NotImplementedException()
                };

                foreach (var image in images)
                {
                    int widthOffset = options.XAlign switch
                    {
                        XAlign.Left => 0,
                        XAlign.Center => (width - image.Width) / 2,
                        XAlign.Right => width - image.Width,
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
                      + (options.Gap * (images.Count() - 1)); // between images
            width = Math.Max(width, options.MinWidth);

            int height = images.Select(i => i.Height).Max();
            height = Math.Max(height, options.MinHeight);
                        
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int widthOffset = options.XAlign switch
                {
                    XAlign.Left => 0,
                    XAlign.Center => (width - images.Width()) / 2,
                    XAlign.Right => width - images.Width(),
                    _ => throw new NotImplementedException()
                };

                foreach (var image in images)
                {
                    int heightOffset = options.YAlign switch
                    {
                        YAlign.Top => 0,
                        YAlign.Center => (height - image.Height) / 2,
                        YAlign.Bottom => height - image.Height,
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

        public static Bitmap Text(string text)
            => Text(new TextOptions(), text);
        public static Bitmap Text(TextOptions options, string text)
        {
            int width = options.Width;
            int height = options.FontSize + options.FontSize / 2;
            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(text, 
                    new Font("Tahoma", options.FontSize), 
                    options.Color,
                    new RectangleF(0, 0, width, height), 
                    format);
            }

            return bitmap;
        }
    }
}
