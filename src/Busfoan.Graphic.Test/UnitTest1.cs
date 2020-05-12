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
            var image1 = new Bitmap("testimage1.png");
            var image2 = new Bitmap("testimage2.png");

            var options = new MergeOptions()
            {
                Padding = 20,
                Gap = 20
            };

            var image = new ImageUtil().MergeHorizontal(options, image1, image2, image1, image2);
            image.Save("test.png", ImageFormat.Png);
        }
    }
}
