using System.Drawing;
using System.Drawing.Imaging;
using Xunit;

// TODO: change project folder
namespace BusfoanBot.Graphic.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var card1 = new Bitmap("testimage1.png");
            var card2 = new Bitmap("testimage2.png");

            var options = new MergeOptions()
            {
                Padding = Padding.All(20),
                Gap = 20
            };

            var image = new ImageUtil().MergeHorizontal(options, card1, card2, card1, card2);
            image.Save("test.png", ImageFormat.Png);

            options.Padding.Right += options.Gap + card1.Width;
            var image2 = new ImageUtil().MergeHorizontal(options, card1, card2, card1);
            image2.Save("test2.png", ImageFormat.Png);
        }
    }
}
