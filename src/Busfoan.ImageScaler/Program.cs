using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Svg;

namespace Busfoan.ImageScaler
{
    class Program
    {
        static readonly string inputPath = "C:/Dev/busfoan-bot/Assets/Cards/Default";
        static readonly string outputPath = "C:/Dev/busfoan-bot/Assets/Cards/Default/Rendered";

        static void Main(string[] args)
        {
            var inputDirectory = new DirectoryInfo(inputPath);
            var files = Directory.EnumerateFiles(inputDirectory.FullName, "*.svg");

            int count = files.Count();
            Console.WriteLine($"Found {count} svgs.");

            var outputDirectory = Directory.CreateDirectory(outputPath);

            int i = 1;
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                var svg = SvgDocument.Open(file);
                svg.Width = 250;
                svg.Height = 363;

                using (var bitmap = svg.Draw())
                {
                    bitmap.Save($"{outputDirectory.FullName}/{fileName}.png", ImageFormat.Png);
                }

                Console.WriteLine($"[{i++}/{count}] Proceed {fileName}.svg");
            }
        }
    }
}
