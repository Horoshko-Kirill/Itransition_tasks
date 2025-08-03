using System;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;

namespace BookStore.Generation
{
    public class GenerationPicture
    {
        public static string GeneratePictureUri(int userSeed, string lang, int index, string title, string[] authors, int width = 300, int height = 450)
        {
            string composite = $"{userSeed}|{lang}|{index}|{title}|{string.Join(",", authors)}";
            byte[] hash = GenerationHashKey.GetHashByteKey(composite);


            int seed = BitConverter.ToInt32(hash, 0);
            var rand = new Random(seed);


            var bgColor = Color.FromRgb(
                (byte)(hash[0] * 0.5),
                (byte)(hash[1] * 0.5),
                (byte)(hash[2] * 0.5));

            using var img = new Image<Rgba32>(width, height);

            FontFamily family = SystemFonts.Collection.Families.First();

            Font titleFont = family.CreateFont(24, FontStyle.Bold);
            Font authorFont = family.CreateFont(20, FontStyle.Italic);

            img.Mutate(ctx =>
            {
           
                ctx.Fill(bgColor);


                int shapesCount = rand.Next(5, 20);
                for (int i = 0; i < shapesCount; i++)
                {
   
                    var shapeColor = new Rgba32(
                        (byte)rand.Next(256),
                        (byte)rand.Next(256),
                        (byte)rand.Next(256),
                        (byte)rand.Next(50, 150));

                    float x = rand.Next(width);
                    float y = rand.Next(height);
                    float size = rand.Next(30, 100);

                    switch (rand.Next(6))
                    {
                        case 0: 
                            ctx.Fill(new Color(shapeColor), new RectangleF(x, y, size, size * 0.6f));
                            break;
                        case 1:
                            ctx.Fill(new Color(shapeColor), new EllipsePolygon(x, y, size / 2));
                            break;
                        case 2: 
                            var triangle = new Polygon(new PointF[]
                            {
                                new PointF(x, y),
                                new PointF(x + size, y),
                                new PointF(x + size/2, y + size)
                            });
                            ctx.Fill(new Color(shapeColor), triangle);
                            break;
                        case 3: 
                            var star = new Star(x + size / 2, y + size / 2, 5, size / 2, size / 4);
                            ctx.Fill(new Color(shapeColor), star);
                            break;
                        case 4: 
                            var hexagon = new RegularPolygon(x + size / 2, y + size / 2, 6, size / 2);
                            ctx.Fill(new Color(shapeColor), hexagon);
                            break;

                        case 5: 
                            var rect = new RectangularPolygon(x, y, size, size * 0.6f);
                            ctx.Fill(new Color(shapeColor), rect);
                            break;
                    }
                }

      
                var textColor = Color.White;

                var titlePosition = new PointF(10, 10);
                ctx.DrawText(title, titleFont, textColor, titlePosition);

                float authorY = width - 50 + titleFont.Size + 10;
                foreach (var author in authors)
                {
                    ctx.DrawText(author, authorFont, textColor, new PointF(10, authorY));
                    authorY += authorFont.Size + 5;
                }
            });

            using var ms = new System.IO.MemoryStream();
            img.SaveAsPng(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());

            return $"data:image/png;base64,{base64}";
        }
    }
}