using System;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using Bogus.DataSets;
using System.Text;

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

            FontFamily family = GetFontFamily(lang);

            Font titleFont = family.CreateFont(25, FontStyle.Bold);
            Font authorFont = family.CreateFont(20, FontStyle.Italic);

            img.Mutate(ctx =>
            {
           
                ctx.Fill(bgColor);
                
                GenerateRandomFigure(rand, width, height, ref ctx);
                      
                var textColor = Color.White;


                float padding = 20f;
                float maxWidth = width - 2 * padding;
                float y = padding;
                var titlePosition = new PointF(10, 10);
                var titleLines = WrapText(title, titleFont, maxWidth);
                var titleOptions = new TextOptions(titleFont);
                foreach (var line in titleLines)
                {
                    ctx.DrawText(
                        new DrawingOptions(),
                        line,
                        titleFont,
                        Color.White,
                        new PointF(padding, y));

                    y += 20;
                }

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

        public static void GenerateRandomFigure(Random rand, int width, int height, ref IImageProcessingContext ctx)
        {
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
        }

        public static FontFamily GetFontFamily(string lang)
        {
            var languageFonts = new Dictionary<string, string[]>
            {
                ["ja"] = new[] { "MS Gothic", "Meiryo", "Yu Gothic", "MS Mincho" },
                ["ru"] = new[] { "Arial", "Times New Roman", "Calibri", "Verdana" }, 
                ["fr"] = new[] { "Arial", "Times New Roman", "Calibri" }, 
                ["en"] = new[] { "Arial", "Times New Roman", "Helvetica" } 
            };

            if (languageFonts.TryGetValue(lang, out var preferredFonts))
            {
                foreach (var fontName in preferredFonts)
                {
                    if (SystemFonts.TryGet(fontName, out var fontFamily))
                    {
                        return fontFamily;
                    }
                }
            }

            return SystemFonts.Collection.Families.First();
        }

        public static List<string> WrapText(string text, Font font, float maxWidth)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text)) return lines;

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var currentLine = new StringBuilder();
            var options = new TextOptions(font);

            foreach (var word in words)
            {
   
                var wordWidth = TextMeasurer.MeasureSize(word, options).Width;

                if (wordWidth > maxWidth)
                {
           
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                    }

       
                    var parts = SplitLongWord(word, font, maxWidth, options);
                    lines.AddRange(parts);
                }
                else
                {

                    var testLine = currentLine.Length > 0
                        ? currentLine + " " + word
                        : word;

                    var lineWidth = TextMeasurer.MeasureSize(testLine, options).Width;

                    if (lineWidth <= maxWidth)
                    {
                        currentLine.Append(currentLine.Length > 0 ? " " + word : word);
                    }
                    else
                    {
                        if (currentLine.Length > 0)
                            lines.Add(currentLine.ToString());

                        currentLine.Clear();
                        currentLine.Append(word);
                    }
                }
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString());

            return lines;
        }

        private static List<string> SplitLongWord(string word, Font font, float maxWidth, TextOptions options)
        {
            var parts = new List<string>();
            var currentPart = new StringBuilder();

            foreach (char c in word)
            {
                var testPart = currentPart.ToString() + c;
                var partWidth = TextMeasurer.MeasureSize(testPart, options).Width;

                if (partWidth <= maxWidth)
                {
                    currentPart.Append(c);
                }
                else
                {
                    if (currentPart.Length > 0)
                        parts.Add(currentPart.ToString());

                    currentPart.Clear();
                    currentPart.Append(c);
                }
            }

            if (currentPart.Length > 0)
                parts.Add(currentPart.ToString());

            return parts;
        }
    }
}