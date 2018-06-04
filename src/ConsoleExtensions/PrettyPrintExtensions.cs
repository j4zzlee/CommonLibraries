using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleExtensions
{
    public static class PrettyPrintExtensions
    {
        /// <summary>
        /// Pretties the print.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultColor">The default color.</param>
        public static void PrettyPrint(this string value, ConsoleColor defaultColor)
        {
            Write(value + Environment.NewLine, defaultColor);
            Console.ResetColor();
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="colorRegex">The color regex.</param>
        /// <param name="defaultForegroundColor">Default color of the foreground.</param>
        /// <param name="defaultBackgroundColor">Default color of the background.</param>
        private static void SetColor(
            string colorRegex,
            ConsoleColor defaultForegroundColor,
            ConsoleColor defaultBackgroundColor)
        {
            var replaceRegex = new Regex("{([fb]):(\\w+)}");

            // {f:Yellow} => f
            var isBackground = replaceRegex.Replace(colorRegex, "$1").Equals("b");

            // {f:Yellow} => Yellow
            var color = replaceRegex.Replace(colorRegex, "$2");

            // Background color
            if (isBackground)
            {
                Console.BackgroundColor = color.Equals("d")
                    ? defaultBackgroundColor
                    : Enum.GetValues(typeof(ConsoleColor))
                        .Cast<ConsoleColor>()
                        .FirstOrDefault(en => string.Equals(en.ToString(), color, StringComparison.CurrentCultureIgnoreCase));
            }
            else // Foreground color
            {
                Console.ForegroundColor = color.Equals("d")
                    ? defaultForegroundColor
                    : Enum.GetValues(typeof(ConsoleColor))
                        .Cast<ConsoleColor>()
                        .FirstOrDefault(en => string.Equals(en.ToString(), color, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultColor">The default color.</param>
        private static void Write(string value, ConsoleColor defaultColor)
        {
            var writeRegex = new Regex("{[fb]:\\w+}");

            var defaultForegroundColor = defaultColor;
            var defaultBackgroundColor = Console.BackgroundColor;

            var segments = writeRegex.Split(value);
            var colors = writeRegex.Matches(value);
            var characterPassed = 0;

            for (var i = 0; i < segments.Length; i++)
            {
                if (i > 0)
                {
                    SetColor(colors[i - 1].Value, defaultForegroundColor, defaultBackgroundColor);
                }
                else
                {
                    Console.ForegroundColor = defaultForegroundColor;
                    Console.BackgroundColor = defaultBackgroundColor;
                }

                // Only bother writing out, if we have something to write.
                if (segments[i].Length <= 0) continue;

                // Align text
                characterPassed = AlignText(segments, i, characterPassed);
            }

            // Reset everything
            Console.ForegroundColor = defaultForegroundColor;
            Console.BackgroundColor = defaultBackgroundColor;
        }

        /// <summary>
        /// Aligns the text.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="i">The i.</param>
        /// <param name="characterPassed">The character passed.</param>
        /// <returns></returns>
        private static int AlignText(IReadOnlyList<string> segments, int i, int characterPassed)
        {
            var tabRegex = new Regex("{t:\\d+}");
            var replaceTabRegex = new Regex("{t:(\\d+)}");
            var segment = segments[i];
            var parts = tabRegex.Split(segment);
            var tabs = tabRegex.Matches(segment);
            for (var j = 0; j < parts.Length; j++)
            {
                var part = parts[j].Replace("\t", new string(' ', 4));

                characterPassed = part.IndexOf('\n') != -1
                    ? part.Length - part.LastIndexOf('\n')
                    : characterPassed + part.Length;
                if (tabs.Count <= j)
                {
                    Console.Write(part);
                    continue;
                }
                var tab = tabs[j];
                var totalWhitespace = int.Parse(replaceTabRegex.Replace(tab.Value, "$1"));
                Console.Write(part);
                if (totalWhitespace > characterPassed)
                {
                    Console.Write(new string(' ', totalWhitespace - characterPassed));
                }

                characterPassed = 0;
            }
            return characterPassed;
        }
    }
}
