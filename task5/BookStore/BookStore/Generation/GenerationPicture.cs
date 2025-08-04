using System;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Formats.Png;
using System.Text;
using System.Numerics;

namespace BookStore.Generation
{
    public class GenerationPicture
    {
        private static readonly FontFamily FontFamily;
        private static readonly PngEncoder FastPngEncoder;


        static GenerationPicture()
        {
            var collection = new FontCollection();
            FontFamily = collection.Add("fonts/NotoSansJP-VariableFont_wght.ttf");

            FastPngEncoder = new PngEncoder
            {
                CompressionLevel = (PngCompressionLevel)1,
                FilterMethod = PngFilterMethod.None
            };
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
            img.Save(ms, FastPngEncoder);

            
            return $"data:image/png;base64,{Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length)}";
        }

        public static void GenerateRandomFigure(Random rand, int width, int height, ref IImageProcessingContext ctx)
        {
            int linesCount = rand.Next(5, 10);

            for (int i = 0; i < linesCount; i++)
            {
                var lineColor = new Rgba32(
                    (byte)rand.Next(256),
                    (byte)rand.Next(256),
                    (byte)rand.Next(256),
                    (byte)rand.Next(100, 255));

                float x1 = rand.Next(width);
                float y1 = rand.Next(height);
                float x2 = rand.Next(width);
                float y2 = rand.Next(height);

                float thickness = (float)(rand.NextDouble() * 3 + 1); // толщина 1..4

                
                var brush = new SolidBrush(lineColor);

 
                var pathBuilder = new PathBuilder();
                pathBuilder.AddLine(new Vector2(x1, y1), new Vector2(x2, y2));
                var path = pathBuilder.Build();

                ctx.Draw(brush, thickness, path);
            }
        }



        public static List<string> WrapText(string text, Font font, float maxWidth)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text)) return lines;

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var currentLine = new StringBuilder();
            var options = new TextOptions(font);

            var widthCache = new Dictionary<string, float>();

            foreach (var word in words)
            {
                if (!widthCache.TryGetValue(word, out float wordWidth))
                {
                    wordWidth = TextMeasurer.MeasureSize(word, options).Width;
                    widthCache[word] = wordWidth;
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
                    var testLine = currentLine.Length > 0 ? currentLine + " " + word : word;

                    if (!widthCache.TryGetValue(testLine, out float lineWidth))
                    {
                        lineWidth = TextMeasurer.MeasureSize(testLine, options).Width;
                        widthCache[testLine] = lineWidth;
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
