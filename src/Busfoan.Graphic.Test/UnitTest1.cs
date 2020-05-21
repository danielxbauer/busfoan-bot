using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using BusfoanBot.Graphic.Extensions;
using BusfoanBot.Graphic.Models;
using BusfoanBot.Graphic.Services;
using Xunit;
using static BusfoanBot.Graphic.Services.ImageUtil;

// TODO: change project folder
namespace BusfoanBot.Graphic.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1; i++)
            {
                var card1 = new Bitmap("C2.png");
                var card2 = new Bitmap("C3.png");

                var options = new MergeOptions()
                {
                    Padding = Padding.All(20),
                    Gap = 20
                };

                var image = Horizontal(options, card1, card2, card1, card2);
                image.Save("test.png", ImageFormat.Png);
            }

            stopwatch.Stop();
        }

        [Fact]
        public void TestPyramid()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var card1 = new Bitmap("C2.png");
            var card2 = new Bitmap("C3.png");

            var options = new MergeOptions
            {
                Padding = Padding.All(5),
                Gap = 40
            };

            var img1 = Horizontal(options,
                card1, card2, card1, card2,
                card1, card2, card1, card2);

            var img2 = Horizontal(options, card1, card2, card1);
            var img3 = Horizontal(options, card1, card2);
            var img4 = Horizontal(options, card1);

            var pyramid = Vertical(new MergeOptions { Direction = Direction.Center },
                img1, img2, img3, img4);

            img1.Save("img1.png", ImageFormat.Png);
            img2.Save("img2.png", ImageFormat.Png);
            img3.Save("img3.png", ImageFormat.Png);
            img4.Save("img4.png", ImageFormat.Png);

            pyramid.Save("pyramid.png", ImageFormat.Png);

            stopwatch.Stop();
        }

        [Fact]
        public void TestHorizontal()
        {
            var i100 = new Bitmap("100x100.png");
            var i200 = new Bitmap("200x200.png");

            var hStart = Horizontal(new MergeOptions { Direction = Direction.Start }, i100, i200);
            hStart.Save("h_1_start.png");

            var hCenter = Horizontal(new MergeOptions { Direction = Direction.Center }, i100, i200);
            hCenter.Save("h_2_center.png");

            var hEnd = Horizontal(new MergeOptions { Direction = Direction.End }, i100, i200);
            hEnd.Save("h_3_end.png");
        }

        [Fact]
        public void TestVertical()
        {
            var i100 = new Bitmap("100x100.png");
            var i200 = new Bitmap("200x200.png");

            var vStart = Vertical(new MergeOptions { Direction = Direction.Start }, i100, i200);
            vStart.Save("v_1_start.png");

            var vCenter = Vertical(new MergeOptions { Direction = Direction.Center }, i100, i200);
            vCenter.Save("v_2_center.png");

            var vEnd = Vertical(new MergeOptions { Direction = Direction.End }, i100, i200);
            vEnd.Save("v_3_end.png");
        }
    }
}
