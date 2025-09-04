using System.Globalization;
using System.Text.RegularExpressions;
using System;
using CourseWork.Models;
using CourseWork.Models.Enums;

namespace CourseWork.Services
{
    public class CustomIdService
    {
        public string Generate(CustomIdFormat format, int lastSequenceNumber)
        {
            string customId = "";

            foreach (var el in format.Elements)
            {
                switch(el.Type)
                {

                    case CustomIdElementType.FixedText:
                        customId += el.FixedValue;
                        break;

                    case CustomIdElementType.Random6Digit:
                        customId += new Random().Next(0, 999999).ToString("D6");
                        break;

                    case CustomIdElementType.Random9Digit:
                        customId += new Random().Next(0, 999_999_999).ToString("D9");
                        break;

                    case CustomIdElementType.Random20Bit:
                        customId += new Random().Next(0, 1 << 20).ToString();
                        break;

                    case CustomIdElementType.Random32Bit:
                        customId += new Random().Next();
                        break;

                    case CustomIdElementType.Guid:
                        customId += Guid.NewGuid().ToString();
                        break;

                    case CustomIdElementType.DateTime:
                        if (!string.IsNullOrEmpty(el.FixedValue))
                            customId += DateTime.UtcNow.ToString(el.FixedValue);
                        else
                            customId += DateTime.UtcNow.ToString("yyyyMMdd");
                        break;

                    case CustomIdElementType.Sequence:
                        if (!string.IsNullOrEmpty(el.FixedValue))
                            customId += (lastSequenceNumber + 1).ToString(el.FixedValue);
                        else
                            customId += (lastSequenceNumber + 1).ToString("D4");
                        break;
                }
            }

            return customId;
        }


        public bool Check(string customId, CustomIdFormat format)
        {
            int pos = 0;

            foreach (var el in format.Elements)
            {
                switch (el.Type)
                {
                    case CustomIdElementType.FixedText:
                        if (customId.Substring(pos, el.FixedValue.Length) != el.FixedValue)
                            return false;
                        pos += el.FixedValue.Length;
                        break;

                    case CustomIdElementType.Random6Digit:
                        if (!Regex.IsMatch(customId.Substring(pos, 6), @"^\d{6}$"))
                            return false;
                        pos += 6;
                        break;

                    case CustomIdElementType.Random9Digit:
                        if (!Regex.IsMatch(customId.Substring(pos, 9), @"^\d{9}$"))
                            return false;
                        pos += 9;
                        break;

                    case CustomIdElementType.Random20Bit:
          
                        var r20 = customId.Substring(pos, 6); 
                        if (!int.TryParse(r20, out int num20) || num20 < 0 || num20 >= (1 << 20))
                            return false;
                        pos += r20.Length;
                        break;

                    case CustomIdElementType.Random32Bit:
                        var r32 = customId.Substring(pos, 10); 
                        if (!int.TryParse(r32, out _))
                            return false;
                        pos += r32.Length;
                        break;

                    case CustomIdElementType.Guid:
                        var guidStr = customId.Substring(pos, 36);
                        if (!Guid.TryParse(guidStr, out _))
                            return false;
                        pos += 36;
                        break;

                    case CustomIdElementType.DateTime:
                        string fmt = string.IsNullOrEmpty(el.FixedValue) ? "yyyyMMdd" : el.FixedValue;
                        int len = DateTime.UtcNow.ToString(fmt).Length;
                        var datePart = customId.Substring(pos, len);
                        if (!DateTime.TryParseExact(datePart, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                            return false;
                        pos += len;
                        break;

                    case CustomIdElementType.Sequence:
                        string seqFmt = string.IsNullOrEmpty(el.FixedValue) ? "D4" : el.FixedValue;
                        int width = int.Parse(seqFmt.Substring(1));
                        var seqPart = customId.Substring(pos, width);
                        if (!Regex.IsMatch(seqPart, @"^\d{" + width + "}$"))
                            return false;
                        pos += width;
                        break;
                }
            }

            return pos == customId.Length;
        }
    }
}
