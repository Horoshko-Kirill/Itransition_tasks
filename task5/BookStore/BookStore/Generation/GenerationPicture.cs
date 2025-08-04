using System;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using System.Text;

namespace BookStore.Generation
{
    public class GenerationPicture
    {
        private static readonly FontFamily FontFamily;

        // Статический конструктор: загружаем шрифт один раз
        static GenerationPicture()
        {
            var collection = new FontCollection();
            FontFamily = collection.Add("fonts/NotoSansJP-VariableFont_wght.ttf");
        }

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

            Font titleFont = FontFamily.CreateFont(25, FontStyle.Bold);
            Font authorFont = FontFamily.CreateFont(20, FontStyle.Italic);

            img.Mutate(ctx =>
            {
                ctx.Fill(bgColor);
                GenerateRandomFigure(rand, width, height, ref ctx);

                float padding = 20f;
                float maxWidth = width - 2 * padding;
                float y = padding;

                var titleLines = WrapText(title, titleFont, maxWidth);
                foreach (var line in titleLines)
                {
                    ctx.DrawText(line, titleFont, Color.White, new PointF(padding, y));
                    y += 20;
                }

                float authorY = width - 50 + titleFont.Size + 10;
                foreach (var author in authors)
                {
                    ctx.DrawText(author, authorFont, Color.White, new PointF(10, authorY));
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
                            new PointF(x + size / 2, y + size)
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

        public static List<string> WrapText(string text, Font font, float maxWidth)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text)) return lines;

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var currentLine = new StringBuilder();
            var options = new TextOptions(font);

            // Кэш размеров слов
            var wordWidthCache = new Dictionary<string, float>();

            foreach (var word in words)
            {
                if (!wordWidthCache.TryGetValue(word, out float wordWidth))
                {
                    wordWidth = TextMeasurer.MeasureSize(word, options).Width;
                    wordWidthCache[word] = wordWidth;
                }

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

                    if (!wordWidthCache.TryGetValue(testLine, out float lineWidth))
                    {
                        lineWidth = TextMeasurer.MeasureSize(testLine, options).Width;
                        wordWidthCache[testLine] = lineWidth;
                    }

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
