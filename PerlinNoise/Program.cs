using System;
using System.Xml.Serialization;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PerlinNoise
{
	class Program
	{
		static void Main(string[] args)
        {
            Image<Rgb24> image = new(1000, 1000);
            Perlin2D perlin = new();

            for(int x = 1; x < image.Width; x++)
            for (int y = 1; y < image.Width; y++)
            {
                byte color = Convert.ToByte((perlin.Noise((x + 0.5F)/100F, (y + 0.5F)/100F, 20) + 1F) * 127F);
                image[x, y] = new Rgb24(color, 0, 0);
            }

            image.Save("image.png");
        }
    }
}
