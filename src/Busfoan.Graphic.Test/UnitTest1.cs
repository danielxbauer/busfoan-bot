using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using BusfoanBot.Graphic.Extensions;
using BusfoanBot.Graphic.Models;
using BusfoanBot.Graphic.Services;
using Xunit;

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

                var image = ImageUtil.MergeHorizontal(new[]
                {
                    card1.WithPadding(Padding.TopPadding(50)),
                    card2.WithPadding(Padding.TopPadding(50)),
                    card1.WithPadding(Padding.TopPadding(50)),
                    card2.WithNoPadding()
                }, options);

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

            var img1 = ImageUtil.MergeHorizontal(new[]
            {
                card1.WithNoPadding(), 
                card2.WithNoPadding(), 
                card1.WithNoPadding(), 
                card2.WithNoPadding()
            }, new MergeOptions()
            {
                Padding = Padding.All(20),
                Gap = 20
            });

            var img2 = ImageUtil.MergeHorizontal(new[]
            {
                card1.WithNoPadding(),
                card2.WithNoPadding(),
                card1.WithNoPadding()
            }, new MergeOptions()
            {
                Padding = Padding.All(20),
                Gap = 20
            });

            var img3 = ImageUtil.MergeHorizontal(new[]
            {
                card1.WithNoPadding(),
                card2.WithNoPadding()
            }, new MergeOptions()
            {
                Padding = Padding.All(20),
                Gap = 20
            });

            var img4 = ImageUtil.MergeHorizontal(new[]
            {
                card1.WithNoPadding()
            }, new MergeOptions()
            {
                Padding = Padding.All(20),
                Gap = 20
            });

            var pyramid = ImageUtil.MergeVertical(new[]
            {
                img4.WithNoPadding(),
                img3.WithNoPadding(),
                img2.WithNoPadding(),
                img1.WithNoPadding()
            }, new MergeOptions());


            img1.Save("img1.png", ImageFormat.Png);
            img2.Save("img2.png", ImageFormat.Png);

            pyramid.Save("pyramid.png", ImageFormat.Png);

            stopwatch.Stop();
        }
    }
}
