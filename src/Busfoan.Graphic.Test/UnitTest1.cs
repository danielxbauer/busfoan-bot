using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using BusfoanBot.Graphic.Extensions;
using BusfoanBot.Graphic.Models;
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
    }
}
