using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CourseWork.Models;
using CourseWork.Models.Enums;

namespace CourseWork.Services
{
    public class CustomIdService
    {
        private readonly string _separator;

        public CustomIdService(string separator = "--")
        {
            _separator = separator;
        }

        public string Generate(CustomIdFormat format, int lastSequenceNumber)
        {
            var parts = new List<string>();

            foreach (var el in format.Elements)
            {
                switch (el.Type)
                {
                    case CustomIdElementType.FixedText:
                        parts.Add(el.FixedValue);
                        break;

                    case CustomIdElementType.Random6Digit:
                        parts.Add(new Random().Next(0, 999999).ToString("D6"));
                        break;

                    case CustomIdElementType.Random9Digit:
                        parts.Add(new Random().Next(0, 999_999_999).ToString("D9"));
                        break;

                    case CustomIdElementType.Random20Bit:
                        parts.Add(new Random().Next(0, 1 << 20).ToString());
                        break;

                    case CustomIdElementType.Random32Bit:
                        parts.Add(new Random().Next().ToString());
                        break;

                    case CustomIdElementType.Guid:
                        parts.Add(Guid.NewGuid().ToString()); 
                        break;

                    case CustomIdElementType.DateTime:
                        parts.Add(!string.IsNullOrEmpty(el.FixedValue)
                            ? DateTime.UtcNow.ToString(el.FixedValue)
                            : DateTime.UtcNow.ToString("yyyyMMdd"));
                        break;

                    case CustomIdElementType.Sequence:
                        parts.Add(!string.IsNullOrEmpty(el.FixedValue)
                            ? (lastSequenceNumber + 1).ToString(el.FixedValue)
                            : (lastSequenceNumber + 1).ToString("D4"));
                        break;
                }
            }

            return string.Join(_separator, parts);
        }

        public bool Check(string customId, CustomIdFormat format)
        {
            var parts = customId.Split(_separator);

            if (parts.Length != format.Elements.Count)
                return false;

            for (int i = 0; i < format.Elements.Count; i++)
            {
                var el = format.Elements[i];
                var part = parts[i];

                switch (el.Type)
                {
                    case CustomIdElementType.FixedText:
                        if (part != el.FixedValue) return false;
                        break;

                    case CustomIdElementType.Random6Digit:
                        if (!Regex.IsMatch(part, @"^\d{6}$")) return false;
                        break;

                    case CustomIdElementType.Random9Digit:
                        if (!Regex.IsMatch(part, @"^\d{9}$")) return false;
                        break;

                    case CustomIdElementType.Random20Bit:
                        if (!int.TryParse(part, out int num20) || num20 < 0 || num20 >= (1 << 20))
                            return false;
                        break;

                    case CustomIdElementType.Random32Bit:
                        if (!int.TryParse(part, out _)) return false;
                        break;

                    case CustomIdElementType.Guid:
             
                        if (!Regex.IsMatch(part, @"^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$"))
                            return false;
                        break;

                    case CustomIdElementType.DateTime:
                        string fmt = string.IsNullOrEmpty(el.FixedValue) ? "yyyyMMdd" : el.FixedValue;
                        if (!DateTime.TryParseExact(part, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                            return false;
                        break;

                    case CustomIdElementType.Sequence:
                        string seqFmt = string.IsNullOrEmpty(el.FixedValue) ? "D4" : el.FixedValue;
                        int width = int.Parse(seqFmt.Substring(1));
                        if (!Regex.IsMatch(part, @"^\d{" + width + "}$")) return false;
                        break;
                }
            }

            return true;
        }
    }
}
